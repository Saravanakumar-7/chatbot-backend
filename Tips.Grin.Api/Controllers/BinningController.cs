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
        public BinningController(IGrinPartsRepository grinPartsRepository, IGrinRepository grinRepository,IBinningRepository binningRepository, IBinningItemsRepository binningItemsRepository,ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _logger = logger;
            _binningRepository = binningRepository;
            _binningItemsRepository = binningItemsRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _grinRepository = grinRepository;
            _grinPartsRepository = grinPartsRepository;
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
                    var result = _mapper.Map<IEnumerable<BinningDto>>(binningDetailsByGrinNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
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
                        binningItems.BinningLocations = _mapper.Map<List<BinningLocation>>(binningItemdto[i].BinningLocations);
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

                var binningCreation = _mapper.Map<Binning>(binningPostDto);
                var binningsDto = binningPostDto.BinningItems;
                
                var binningItemList = new List<BinningItems>();
                if (binningsDto != null)
                {
                    for (int i = 0; i < binningsDto.Count; i++)
                    {

                        BinningItems binningItems = _mapper.Map<BinningItems>(binningsDto[i]);
                        binningItems.BinningLocations = _mapper.Map<List<BinningLocation>>(binningsDto[i].BinningLocations);
                        binningItemList.Add(binningItems);


                    }
                }
                binningCreation.BinningItems = binningItemList;

              binningDetails = await  _binningRepository.CreateBinning(binningCreation);


                _binningRepository.SaveAsync();

                // Inventory Update Code
                string grinPartId = "";
               
                if (binningsDto != null)
                {
                    for (int i = 0; i < binningsDto.Count; i++)
                    {
                        var binningLocations = binningsDto[i].BinningLocations;
                        for (int j = 0; j < binningLocations.Count; j++)
                        {
                            if (j == 0)
                            {
                                var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"], "GetInventoryDetailsByGrinNo?", "GrinNo=", binningCreation.GrinNumber, "&ItemNumber=", binningsDto[i].ItemNumber, "&ProjectNumber=", binningLocations[i].ProjectNumber));
                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                dynamic inventoryObject = inventoryObjectData.data;
                                inventoryObject.Balance_Quantity = binningLocations[j].Qty;
                                inventoryObject.Warehouse = binningLocations[j].Warehouse;
                                inventoryObject.Location = binningLocations[j].Location;
                                inventoryObject.ReferenceIDFrom = "GRIN";
                                grinPartId =  inventoryObject.referenceID;
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"], "UpdateInventory/", inventoryObject.id), data);
                            }
                            else
                            {
                                var grinId = binningsDto[i].GrinPartId;
                                var grinDetails =await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinId);
                                dynamic inventoryObject = new ExpandoObject();
                                inventoryObject.PartNumber = binningsDto[i].ItemNumber;
                                inventoryObject.MftrPartNumber = grinDetails.MftrItemNumber;
                                inventoryObject.Description = grinDetails.ItemDescription;
                                inventoryObject.ProjectNumber = binningLocations[i].ProjectNumber;
                                inventoryObject.Balance_Quantity = binningLocations[j].Qty;
                                inventoryObject.UOM = grinDetails.UOM;
                                inventoryObject.IsStockAvailable = true;
                                inventoryObject.Warehouse = binningLocations[j].Warehouse;
                                inventoryObject.Location = binningLocations[j].Location;
                                inventoryObject.GrinNo = binningCreation.GrinNumber;
                                inventoryObject.GrinPartId = Convert.ToInt32(grinPartId);
                                inventoryObject.PartType = "PurchasePart";
                                inventoryObject.ReferenceID = grinPartId;
                                inventoryObject.ReferenceIDFrom = "GRIN";

                                var json = JsonConvert.SerializeObject(inventoryObject);
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
                            binningItemDtos.BinningLocations = _mapper.Map<List<BinningLocationDto>>(grinDetails);
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
        public async Task<IActionResult> GetAllBinningItems([FromQuery] PagingParameter pagingParameter,[FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<BinningItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningItemsDto>>();

            try
            {
                var getAllBinningItems = await _binningItemsRepository.GetAllBinningItems(pagingParameter,searchParams);

                var metadata = new
                {
                    getAllBinningItems.TotalCount,
                    getAllBinningItems.PageSize,
                    getAllBinningItems.CurrentPage,
                    getAllBinningItems.HasNext,
                    getAllBinningItems.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

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
    }
}