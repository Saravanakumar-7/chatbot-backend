using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Repository;
using Entities;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OpenGrinForBinningController : ControllerBase
    {
        private IOpenGrinForBinningRepository _openGrinForBinningRepository;
        private IOpenGrinForBinningItemsRepository _openGrinForBinningItemsRepository;
        private IOpenGrinForGrinRepository _openGrinForGrinRepository;
        private IOpenGrinForGrinItemRepository _openGrinForGrinItemRepository;
        private IOpenGrinForIQCRepository _openGrinForIQCRepository;
        private IOpenGrinForIQCItemRepository _openGrinForIQCItemRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public OpenGrinForBinningController(IOpenGrinForIQCItemRepository openGrinForIQCItemRepository, IOpenGrinForIQCRepository openGrinForIQCRepository, IOpenGrinForGrinItemRepository openGrinForGrinItemRepository, IOpenGrinForGrinRepository openGrinForGrinRepository, IOpenGrinForBinningItemsRepository openGrinForBinningItemsRepository, IOpenGrinForBinningRepository openGrinForBinningRepository, IBinningLocationRepository binningLocationRepository, IHttpClientFactory clientFactory, IIQCConfirmationItemsRepository iQCConfirmationItemsRepository, IIQCConfirmationRepository iQCConfirmationRepository, IGrinPartsRepository grinPartsRepository, IGrinRepository grinRepository, IBinningRepository binningRepository, IBinningItemsRepository binningItemsRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _openGrinForBinningRepository = openGrinForBinningRepository;
            _openGrinForBinningItemsRepository = openGrinForBinningItemsRepository;
            _openGrinForGrinRepository = openGrinForGrinRepository;
            _openGrinForGrinItemRepository = openGrinForGrinItemRepository;
            _openGrinForIQCRepository = openGrinForIQCRepository;
            _openGrinForIQCItemRepository = openGrinForIQCItemRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenGrinForBinningDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<OpenGrinForBinningDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinForBinningDto>>();

            try
            {
                var openGrinForBinningDetails = await _openGrinForBinningRepository.GetAllOpenGrinForBinningDetails(pagingParameter, searchParams);
                var metadata = new
                {
                    openGrinForBinningDetails.TotalCount,
                    openGrinForBinningDetails.PageSize,
                    openGrinForBinningDetails.CurrentPage,
                    openGrinForBinningDetails.HasNext,
                    openGrinForBinningDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all OpenGrinForBinning");
                var result = _mapper.Map<IEnumerable<OpenGrinForBinningDto>>(openGrinForBinningDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrinForBinning";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllOpenGrinForBinningDetails,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpenGrinForBinningDetailsbyId(int id)
        {
            ServiceResponse<OpenGrinForBinningDto> serviceResponse = new ServiceResponse<OpenGrinForBinningDto>();

            try
            {
                var openGrinForBinningDetails = await _openGrinForBinningRepository.GetOpenGrinForBinningDetailsbyId(id);
                if (openGrinForBinningDetails == null)
                {
                    _logger.LogError($"OpenGrinForBinning details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrinForBinning details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned OpenGrinForBinning with id: {id}");
                    List<OpenGrinForBinningItemsDto> openGrinForBinningItemsList = new List<OpenGrinForBinningItemsDto>();
                    var openGrinNumber = openGrinForBinningDetails.OpenGrinNumber;
                    var openGrinForBinningItems = openGrinForBinningDetails.OpenGrinForBinningItems;
                    var openGrinForGrinDetails = await _openGrinForGrinRepository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                    var openGrinForBinningDetailsDto = _mapper.Map<OpenGrinForBinningDto>(openGrinForGrinDetails);

                    if (openGrinForBinningDetails.OpenGrinForBinningItems != null)
                    {
                        foreach (var openGrinForBinningItem in openGrinForBinningDetails.OpenGrinForBinningItems)
                        {
                            foreach (var openGrinForGrinItem in openGrinForGrinDetails.OpenGrinForGrinItems)
                            {
                                if (openGrinForBinningItem.OpenGrinForGrinItemId == openGrinForGrinItem.Id)
                                {
                                    OpenGrinForBinningItemsDto openGrinForBinningItemsDtos = _mapper.Map<OpenGrinForBinningItemsDto>(openGrinForGrinItem);
                                    var openGrinForBinningLocationDetails = _mapper.Map<List<OpenGrinForBinningLocationsDto>>(openGrinForBinningItem.OpenGrinForBinningLocations);
                                    openGrinForBinningItemsDtos.OpenGrinForBinningLocations = openGrinForBinningLocationDetails;
                                    openGrinForBinningItemsDtos.ReceivedQty = openGrinForGrinItem.Qty;
                                    openGrinForBinningItemsDtos.AcceptedQty = openGrinForGrinItem.AcceptedQty;
                                    openGrinForBinningItemsDtos.RejectedQty = openGrinForGrinItem.RejectedQty;
                                    openGrinForBinningItemsList.Add(openGrinForBinningItemsDtos);
                                    break;
                                }
                            }
                        }
                    }

                    openGrinForBinningDetailsDto.OpenGrinForBinningItems = openGrinForBinningItemsList;
                    serviceResponse.Data = openGrinForBinningDetailsDto;
                    serviceResponse.Message = $"Returned OpenGrinForBinningDetailsById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOpenGrinForBinningDetailsbyId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOpenGrinForBinning([FromBody] OpenGrinForBinningPostDto openGrinForBinningPostDto)
        {
            ServiceResponse<OpenGrinForBinning> serviceResponse = new ServiceResponse<OpenGrinForBinning>();
            OpenGrinForBinning binningDetails = null;
            try
            {
                if (openGrinForBinningPostDto == null)
                {
                    _logger.LogError("OpenGrinForBinning details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenGrinForBinning details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid OpenGrinForBinning details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var openGrinForBinningDetail = _mapper.Map<OpenGrinForBinning>(openGrinForBinningPostDto);
                var openGrinForBinningItemsDto = openGrinForBinningPostDto.OpenGrinForBinningItems;
                var openGrinNumber = openGrinForBinningDetail.OpenGrinNumber;
                var openGrinForBinningItemlist = new List<OpenGrinForBinningItems>();

                HttpStatusCode getInvGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode getItemMas = HttpStatusCode.OK;
                HttpStatusCode createInvTrans = HttpStatusCode.OK;

                var existingOpenGrinForBinningDetails = await _openGrinForBinningRepository.GetExistingOpenGrinForBinningDetailsByOpenGrinNo(openGrinNumber);
                if (existingOpenGrinForBinningDetails != null)
                {
                    if (openGrinForBinningItemsDto != null)
                    {
                        for (int i = 0; i < openGrinForBinningItemsDto.Count; i++)
                        {

                            OpenGrinForBinningItems openGrinForBinningItems = _mapper.Map<OpenGrinForBinningItems>(openGrinForBinningItemsDto[i]);
                            openGrinForBinningItems.OpenGrinForBinningId = existingOpenGrinForBinningDetails.Id;
                            openGrinForBinningItems.OpenGrinForBinningLocations = _mapper.Map<List<OpenGrinForBinningLocations>>(openGrinForBinningItemsDto[i].OpenGrinForBinningLocations);

                            //Updating Binning Status in OpenGrinForBinningItem

                            openGrinForBinningItems.IsOpenGrinForBinningCompleted = true;
                            await _openGrinForBinningItemsRepository.UpdateOpenGrinForBinningItems(openGrinForBinningItems);
                            _openGrinForBinningItemsRepository.SaveAsync();

                            //Updating Binning Status in OpenGrinForGrinItems

                            var openGrinForGrinItemsId = openGrinForBinningItemsDto[i].OpenGrinForGrinItemId;
                            var openGrinForGrinItemDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemsId);
                            openGrinForGrinItemDetails.IsOpenGrinForBinningCompleted = true;
                            await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemDetails);
                            _openGrinForGrinItemRepository.SaveAsync();

                            //Updating Binning Status in OpenGrinForIQCItems

                            var openGrinForIQCItemDetails = await _openGrinForIQCItemRepository.GetOpenGrinForIQCItemsDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemsId);
                            openGrinForIQCItemDetails.IsOpenGrinForBinningCompleted = true;
                            await _openGrinForIQCItemRepository.UpdateOpenGrinForIQCItems(openGrinForIQCItemDetails);
                            _openGrinForIQCItemRepository.SaveAsync();

                            //Updating Binning Status in OpenGrinForGrin

                            var openGrinForGrinPartsBinningStatuscount = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemsBinningStatusCount(openGrinForGrinItemDetails.OpenGrinForGrinId);

                            if (openGrinForGrinPartsBinningStatuscount == 0)
                            {
                                var openGrinForGrinDetails = await _openGrinForGrinRepository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                                openGrinForGrinDetails.IsOpenGrinForBinningCompleted = true;
                                await _openGrinForGrinRepository.UpdateOpenGrinForGrin(openGrinForGrinDetails);
                                _openGrinForGrinRepository.SaveAsync();

                            }

                            //Updating Binning Status in IQC & Binning

                            var openGrinForGrinBinningStatuscount = await _openGrinForGrinRepository.GetOpenGrinForGrinbinningStatusCount(openGrinNumber);

                            if (openGrinForGrinBinningStatuscount > 0)
                            {
                                var openGrinForIQCDetails = await _openGrinForIQCRepository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);
                                openGrinForIQCDetails.IsOpenGrinForBinningCompleted = true;
                                await _openGrinForIQCRepository.UpdateOpenGrinForIQC(openGrinForIQCDetails);


                                var openGrinForBinningDetails = await _openGrinForBinningRepository.GetOpenGrinForBinningDetailsByOpenGrinNo(openGrinNumber);
                                openGrinForBinningDetails.IsOpenGrinForBinningCompleted = true;
                                await _openGrinForBinningRepository.UpdateOpenGrinForBinning(openGrinForBinningDetails);

                            }
                        }
                    }

                    // Inventory Update Code
                    string grinPartId = "";
                    if (openGrinForBinningItemsDto != null)
                    {
                        foreach (var openGrinForBinningsItem in openGrinForBinningItemsDto)
                        {

                            var OpenGrinForGrinItemId = openGrinForBinningsItem.OpenGrinForGrinItemId;


                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();

                            var ItemNumber = openGrinForBinningsItem.ItemNumber;
                            var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                                $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                            request.Headers.Add("Authorization", token);

                            var itemMasterObjectResult = await client.SendAsync(request);
                            if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK) getItemMas = itemMasterObjectResult.StatusCode;
                            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                            var itemMasterObjectData = JsonConvert.DeserializeObject<OpenGrinForBinningItemMasterDetails>(itemMasterObjectString);
                            var itemMasterObject = itemMasterObjectData.data;

                            var openGrinForBinningLocations = openGrinForBinningsItem.OpenGrinForBinningLocations;

                            int j = 0;
                            int k = 0;
                            foreach (var location in openGrinForBinningLocations)
                            {
                                if (j == 0)
                                {

                                    var client1 = _clientFactory.CreateClient();
                                    var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                    var itemNo = openGrinForBinningsItem.ItemNumber;
                                    var encodedItemNo = Uri.EscapeDataString(itemNo);
                                    var openGrinNo = openGrinForBinningDetail.OpenGrinNumber;
                                    var encodedopenGrinNo = Uri.EscapeDataString(openGrinNo);
                                    var openGrinForGrinItemId = openGrinForBinningsItem.OpenGrinForGrinItemId;
                                    var referenceSONumbers = location.ReferenceSONumber;
                                    var encodedReferenceSONumbers = Uri.EscapeDataString(referenceSONumbers);

                                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                        $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedopenGrinNo}&GrinPartsId={openGrinForGrinItemId}&ItemNumber={encodedItemNo}&ProjectNumber={encodedReferenceSONumbers}"));
                                    request1.Headers.Add("Authorization", token1);

                                    var inventoryObjectResult = await client1.SendAsync(request1);
                                    if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;

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

                                    //InventoryTranction Update Code
                                    IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                                    iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                                    iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                                    iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                                    iqcInventoryTranctionDto.Description = inventoryObject.description;
                                    iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                                    iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                    iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                                    iqcInventoryTranctionDto.Warehouse = inventoryObject.warehouse;
                                    iqcInventoryTranctionDto.From_Location = inventoryObject.location;
                                    iqcInventoryTranctionDto.TO_Location = inventoryObject.location;
                                    iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; 
                                    iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                    iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                                    iqcInventoryTranctionDto.ReferenceID = inventoryObject.referenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                    iqcInventoryTranctionDto.ReferenceIDFrom = inventoryObject.referenceIDFrom;
                                    iqcInventoryTranctionDto.GrinMaterialType = "OPGGRIN";
                                    iqcInventoryTranctionDto.ShopOrderNo = "";

                                    var jsonss = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                                    var datass = new StringContent(jsonss, Encoding.UTF8, "application/json");
                                    //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);
                                    var client10 = _clientFactory.CreateClient();
                                    var token10 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request10 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                    "CreateInventoryTranction"))
                                    {
                                        Content = datass
                                    };
                                    request10.Headers.Add("Authorization", token10);

                                    var responses1 = await client10.SendAsync(request10);

                                    if (responses1.StatusCode != HttpStatusCode.OK) createInvTrans = responses1.StatusCode;

                                    j++;
                                }
                                else
                                {

                                    var openGrinForGrinItemsDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(OpenGrinForGrinItemId);
                                    BinningInventoryDtoPost inventoryObjectNew = new BinningInventoryDtoPost();
                                    inventoryObjectNew.PartNumber = openGrinForBinningsItem.ItemNumber;
                                    inventoryObjectNew.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                                    inventoryObjectNew.Description = itemMasterObject.description;
                                    inventoryObjectNew.ProjectNumber = location.ReferenceSONumber;
                                    inventoryObjectNew.Balance_Quantity = location.Qty;
                                    inventoryObjectNew.Max = itemMasterObject.max;
                                    inventoryObjectNew.Min = itemMasterObject.min;
                                    inventoryObjectNew.UOM = openGrinForGrinItemsDetails.UOM;
                                    inventoryObjectNew.Warehouse = location.Warehouse;
                                    inventoryObjectNew.Location = location.Location;
                                    inventoryObjectNew.GrinNo = openGrinForBinningDetail.OpenGrinNumber;
                                    inventoryObjectNew.GrinPartId = OpenGrinForGrinItemId;
                                    inventoryObjectNew.PartType = openGrinForGrinItemsDetails.ItemType; // we have to take parttype from grinparts model;
                                    inventoryObjectNew.ReferenceID = openGrinForBinningDetail.OpenGrinNumber;
                                    inventoryObjectNew.ReferenceIDFrom = "OPGGRIN";
                                    inventoryObjectNew.LotNumber = openGrinForGrinItemsDetails.LotNumber;

                                    var json = JsonConvert.SerializeObject(inventoryObjectNew);
                                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                                    //HttpResponseMessage response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);

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

                                    //InventoryTranction Update Code

                                    BinningInventoryTranctionDto inventoryTranctionObjectNew = new BinningInventoryTranctionDto();
                                    inventoryTranctionObjectNew.PartNumber = inventoryObjectNew.PartNumber;
                                    inventoryTranctionObjectNew.MftrPartNumber = inventoryObjectNew.MftrPartNumber;
                                    inventoryTranctionObjectNew.Description = inventoryObjectNew.Description;
                                    inventoryTranctionObjectNew.ProjectNumber = inventoryObjectNew.ProjectNumber;
                                    inventoryTranctionObjectNew.Issued_Quantity = inventoryObjectNew.Balance_Quantity;
                                    inventoryTranctionObjectNew.UOM = inventoryObjectNew.UOM;
                                    inventoryTranctionObjectNew.Warehouse = inventoryObjectNew.Warehouse;
                                    inventoryTranctionObjectNew.From_Location = "OPGNIQC";
                                    inventoryTranctionObjectNew.TO_Location = inventoryObjectNew.Location;
                                    inventoryTranctionObjectNew.GrinNo = inventoryObjectNew.GrinNo;
                                    inventoryTranctionObjectNew.GrinPartId = inventoryObjectNew.GrinPartId;
                                    inventoryTranctionObjectNew.PartType = inventoryObjectNew.PartType; // we have to take parttype from grinparts model;
                                    inventoryTranctionObjectNew.ReferenceID = inventoryObjectNew.ReferenceID;
                                    inventoryTranctionObjectNew.ReferenceIDFrom = inventoryObjectNew.ReferenceIDFrom;

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
                    }
                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK
                                                                                                                                    && getItemMas == HttpStatusCode.OK)
                    {
                        _openGrinForIQCRepository.SaveAsync();
                        _openGrinForBinningRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create CreateOpenGrinForBinning action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = binningDetails;
                    serviceResponse.Message = "OpenGrinForBinningItems Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (openGrinForBinningItemsDto != null)
                    {
                        for (int i = 0; i < openGrinForBinningItemsDto.Count; i++)
                        {

                            OpenGrinForBinningItems openGrinForBinningItems = _mapper.Map<OpenGrinForBinningItems>(openGrinForBinningItemsDto[i]);
                            openGrinForBinningItems.OpenGrinForBinningLocations = _mapper.Map<List<OpenGrinForBinningLocations>>(openGrinForBinningItemsDto[i].OpenGrinForBinningLocations);
                            openGrinForBinningItems.IsOpenGrinForBinningCompleted = true;
                            openGrinForBinningItemlist.Add(openGrinForBinningItems);

                            //Updating Binning Status in OpenGrinForGrinItems

                            var openGrinForGrinItemsId = openGrinForBinningItemsDto[i].OpenGrinForGrinItemId;
                            var openGrinForGrinItemDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemsId);
                            openGrinForGrinItemDetails.IsOpenGrinForBinningCompleted = true;
                            await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemDetails);
                            _openGrinForGrinItemRepository.SaveAsync();

                            //Updating Binning Status in OpenGrinForIQCItems

                            var openGrinForIQCItemDetails = await _openGrinForIQCItemRepository.GetOpenGrinForIQCItemsDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemsId);
                            openGrinForIQCItemDetails.IsOpenGrinForBinningCompleted = true;
                            await _openGrinForIQCItemRepository.UpdateOpenGrinForIQCItems(openGrinForIQCItemDetails);
                            _openGrinForIQCItemRepository.SaveAsync();
                        }
                    }


                    // Inventory Update Code
                    string grinPartId = "";
                    if (openGrinForBinningItemsDto != null)
                    {
                        foreach (var openGrinForBinningsItem in openGrinForBinningItemsDto)
                        {

                            var OpenGrinForGrinItemId = openGrinForBinningsItem.OpenGrinForGrinItemId;

                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();

                            var ItemNumber = openGrinForBinningsItem.ItemNumber;
                            var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                                $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                            request.Headers.Add("Authorization", token);

                            var itemMasterObjectResult = await client.SendAsync(request);

                            if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK) getItemMas = itemMasterObjectResult.StatusCode;
                            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                            var itemMasterObjectData = JsonConvert.DeserializeObject<OpenGrinForBinningItemMasterDetails>(itemMasterObjectString);
                            var itemMasterObject = itemMasterObjectData.data;

                            var openGrinForBinningLocations = openGrinForBinningsItem.OpenGrinForBinningLocations;

                            int j = 0;
                            int k = 0;
                            foreach (var location in openGrinForBinningLocations)
                            {
                                if (j == 0)
                                {

                                    var client1 = _clientFactory.CreateClient();
                                    var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                    var itemNo = openGrinForBinningsItem.ItemNumber;
                                    var encodedItemNo = Uri.EscapeDataString(itemNo);
                                    var openGrinNo = openGrinForBinningDetail.OpenGrinNumber;
                                    var encodedopenGrinNo = Uri.EscapeDataString(openGrinNo);
                                    var openGrinForGrinItemId = openGrinForBinningsItem.OpenGrinForGrinItemId;
                                    var referenceSONumbers = location.ReferenceSONumber;
                                    var encodedReferenceSONumbers = Uri.EscapeDataString(referenceSONumbers);

                                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                        $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedopenGrinNo}&GrinPartsId={openGrinForGrinItemId}&ItemNumber={encodedItemNo}&ProjectNumber={encodedReferenceSONumbers}"));
                                    request1.Headers.Add("Authorization", token1);

                                    var inventoryObjectResult = await client1.SendAsync(request1);

                                    if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;

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

                                    //InventoryTranction Update Code
                                    IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                                    iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                                    iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                                    iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                                    iqcInventoryTranctionDto.Description = inventoryObject.description;
                                    iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                                    iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                    iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                                    iqcInventoryTranctionDto.Warehouse = inventoryObject.warehouse;
                                    iqcInventoryTranctionDto.From_Location = inventoryObject.location;
                                    iqcInventoryTranctionDto.TO_Location = inventoryObject.location;
                                    iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; 
                                    iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                    iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                                    iqcInventoryTranctionDto.ReferenceID = inventoryObject.referenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                    iqcInventoryTranctionDto.ReferenceIDFrom = inventoryObject.referenceIDFrom;
                                    iqcInventoryTranctionDto.GrinMaterialType = "OPGGRIN";
                                    iqcInventoryTranctionDto.ShopOrderNo = "";

                                    var jsonss = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                                    var datass = new StringContent(jsonss, Encoding.UTF8, "application/json");
                                    //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);
                                    var client10 = _clientFactory.CreateClient();
                                    var token10 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request10 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                    "CreateInventoryTranction"))
                                    {
                                        Content = datass
                                    };
                                    request10.Headers.Add("Authorization", token10);

                                    var responses1 = await client10.SendAsync(request10);

                                    if (responses1.StatusCode != HttpStatusCode.OK) createInvTrans = responses1.StatusCode;

                                    j++;
                                }
                                else
                                {
                                    var openGrinForGrinItemsDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(OpenGrinForGrinItemId);
                                    BinningInventoryDtoPost inventoryObjectNew = new BinningInventoryDtoPost();
                                    inventoryObjectNew.PartNumber = openGrinForBinningsItem.ItemNumber;
                                    inventoryObjectNew.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); 
                                    inventoryObjectNew.Description = itemMasterObject.description;
                                    inventoryObjectNew.ProjectNumber = location.ReferenceSONumber;
                                    inventoryObjectNew.Balance_Quantity = location.Qty;
                                    inventoryObjectNew.Max = itemMasterObject.max;
                                    inventoryObjectNew.Min = itemMasterObject.min;
                                    inventoryObjectNew.UOM = openGrinForGrinItemsDetails.UOM;
                                    inventoryObjectNew.Warehouse = location.Warehouse;
                                    inventoryObjectNew.Location = location.Location;
                                    inventoryObjectNew.GrinNo = openGrinForBinningDetail.OpenGrinNumber;
                                    inventoryObjectNew.GrinPartId = OpenGrinForGrinItemId;
                                    inventoryObjectNew.PartType = openGrinForGrinItemsDetails.ItemType; // we have to take parttype from grinparts model;
                                    inventoryObjectNew.ReferenceID = openGrinForBinningDetail.OpenGrinNumber;
                                    inventoryObjectNew.ReferenceIDFrom = "OPGGRIN";
                                    inventoryObjectNew.LotNumber = openGrinForGrinItemsDetails.LotNumber;

                                    var json = JsonConvert.SerializeObject(inventoryObjectNew);
                                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                                    //HttpResponseMessage response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);

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

                                    //InventoryTranction Update Code

                                    BinningInventoryTranctionDto inventoryTranctionObjectNew = new BinningInventoryTranctionDto();
                                    inventoryTranctionObjectNew.PartNumber = inventoryObjectNew.PartNumber;
                                    inventoryTranctionObjectNew.MftrPartNumber = inventoryObjectNew.MftrPartNumber;
                                    inventoryTranctionObjectNew.Description = inventoryObjectNew.Description;
                                    inventoryTranctionObjectNew.ProjectNumber = inventoryObjectNew.ProjectNumber;
                                    inventoryTranctionObjectNew.Issued_Quantity = inventoryObjectNew.Balance_Quantity;
                                    inventoryTranctionObjectNew.UOM = inventoryObjectNew.UOM;
                                    inventoryTranctionObjectNew.Warehouse = inventoryObjectNew.Warehouse;
                                    inventoryTranctionObjectNew.From_Location = "OPGNIQC";
                                    inventoryTranctionObjectNew.TO_Location = inventoryObjectNew.Location;
                                    inventoryTranctionObjectNew.GrinNo = inventoryObjectNew.GrinNo;
                                    inventoryTranctionObjectNew.GrinPartId = inventoryObjectNew.GrinPartId;
                                    inventoryTranctionObjectNew.PartType = inventoryObjectNew.PartType; // we have to take parttype from grinparts model;
                                    inventoryTranctionObjectNew.ReferenceID = inventoryObjectNew.ReferenceID;
                                    inventoryTranctionObjectNew.ReferenceIDFrom = inventoryObjectNew.ReferenceIDFrom;

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
                    }

                    //Updating Binning Status in OpenGrinForBinning Main Level

                    openGrinForBinningDetail.OpenGrinForBinningItems = openGrinForBinningItemlist;
                    openGrinForBinningDetail.IsOpenGrinForBinningCompleted = true;
                    await _openGrinForBinningRepository.CreateOpenGrinForBinning(openGrinForBinningDetail);

                    //Updating Binning Status in OpenGrinForGrin Main Level

                    var openGrinForGrinDetails = await _openGrinForGrinRepository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                    openGrinForGrinDetails.IsOpenGrinForBinningCompleted = true;
                    await _openGrinForGrinRepository.UpdateOpenGrinForGrin(openGrinForGrinDetails);

                    //Updating Binning Status in OpenGrinForIQC Main Level

                    var openGrinForIQCDetails = await _openGrinForIQCRepository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);
                    openGrinForIQCDetails.IsOpenGrinForBinningCompleted = true;
                    await _openGrinForIQCRepository.UpdateOpenGrinForIQC(openGrinForIQCDetails);

                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && createInv == HttpStatusCode.OK && getItemMas == HttpStatusCode.OK
                                                                                                                            && createInvTrans == HttpStatusCode.OK)
                    {
                        _openGrinForGrinRepository.SaveAsync();
                        _openGrinForIQCRepository.SaveAsync();
                        _openGrinForBinningRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create CreateOpenGrinForBinning action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = binningDetails;
                    serviceResponse.Message = "OpenGrinForBinning Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Something went wrong inside Create CreateOpenGrinForBinning action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal Server Error");


            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOpenGrinForBinningItems([FromBody] OpenGrinForBinningSaveDto openGrinForBinningSaveDto)
        {
            ServiceResponse<OpenGrinForBinningSaveDto> serviceResponse = new ServiceResponse<OpenGrinForBinningSaveDto>();
            try
            {
                if (openGrinForBinningSaveDto == null)
                {
                    _logger.LogError("OpenGrinForBinning details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenGrinForBinning details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid OpenGrinForBinning details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var openGrinForBinningDetail = new OpenGrinForBinning();

                openGrinForBinningDetail.OpenGrinNumber = openGrinForBinningSaveDto.OpenGrinNumber;

                List<OpenGrinForBinningItems>? binningItemsEntityList = new List<OpenGrinForBinningItems>();
                OpenGrinForBinningItems openGrinForBinningItemsEntity = new OpenGrinForBinningItems();
                openGrinForBinningItemsEntity.ItemNumber = openGrinForBinningSaveDto.OpenGrinForBinningItems.ItemNumber;
                openGrinForBinningItemsEntity.OpenGrinForGrinItemId = openGrinForBinningSaveDto.OpenGrinForBinningItems.OpenGrinForGrinItemId;
                openGrinForBinningItemsEntity.OpenGrinForBinningLocations = _mapper.Map<List<OpenGrinForBinningLocations>>(openGrinForBinningSaveDto.OpenGrinForBinningItems.OpenGrinForBinningLocations);
                binningItemsEntityList.Add(openGrinForBinningItemsEntity);
                openGrinForBinningDetail.OpenGrinForBinningItems = binningItemsEntityList;

                HttpStatusCode getInvGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode getItemMas = HttpStatusCode.OK;
                HttpStatusCode createInvTrans = HttpStatusCode.OK;

                var openGrinForBinningItemsDto = openGrinForBinningSaveDto.OpenGrinForBinningItems;
                var openGrinForBinningItems = openGrinForBinningDetail.OpenGrinForBinningItems[0];
                var openGrinNumber = openGrinForBinningDetail.OpenGrinNumber;
                var existingBinningDetails = await _openGrinForBinningRepository.GetExistingOpenGrinForBinningDetailsByOpenGrinNo(openGrinNumber);

                if (existingBinningDetails != null)
                {
                    //Updating Binning Status in OpenGrinForBinningItem

                    openGrinForBinningItems.OpenGrinForBinningId = existingBinningDetails.Id;
                    openGrinForBinningItems.IsOpenGrinForBinningCompleted = true;
                    await _openGrinForBinningItemsRepository.CreateOpenGrinForBinningItems(openGrinForBinningItems);
                    _openGrinForBinningItemsRepository.SaveAsync();

                    //Updating Binning Status in OpenGrinForGrinItems

                    var openGrinForGrinItemId = openGrinForBinningItemsDto.OpenGrinForGrinItemId;
                    var openGrinForGrinItemDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);
                    openGrinForGrinItemDetails.IsOpenGrinForBinningCompleted = true;
                    await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemDetails);
                    _openGrinForGrinItemRepository.SaveAsync();

                    //Updating Binning Status in OpenGrinForIQCItems

                    var openGrinForIQCItemDetails = await _openGrinForIQCItemRepository.GetOpenGrinForIQCItemsDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);
                    openGrinForIQCItemDetails.IsOpenGrinForBinningCompleted = true;
                    await _openGrinForIQCItemRepository.UpdateOpenGrinForIQCItems(openGrinForIQCItemDetails);
                    _openGrinForIQCItemRepository.SaveAsync();

                    //Updating Binning Status in OpenGrinForGrin

                    var openGrinForGrinPartsBinningStatuscount = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemsBinningStatusCount(openGrinForGrinItemDetails.OpenGrinForGrinId);

                    if (openGrinForGrinPartsBinningStatuscount == 0)
                    {
                        var openGrinForGrinDetails = await _openGrinForGrinRepository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                        openGrinForGrinDetails.IsOpenGrinForBinningCompleted = true;
                        await _openGrinForGrinRepository.UpdateOpenGrinForGrin(openGrinForGrinDetails);
                        _openGrinForGrinRepository.SaveAsync();

                    }

                    //Updating Binning Status in IQC & Binning

                    var openGrinForGrinBinningStatuscount = await _openGrinForGrinRepository.GetOpenGrinForGrinbinningStatusCount(openGrinNumber);

                    if (openGrinForGrinBinningStatuscount > 0)
                    {
                        var openGrinForIQCDetails = await _openGrinForIQCRepository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);
                        openGrinForIQCDetails.IsOpenGrinForBinningCompleted = true;
                        await _openGrinForIQCRepository.UpdateOpenGrinForIQC(openGrinForIQCDetails);


                        var openGrinForBinningDetails = await _openGrinForBinningRepository.GetOpenGrinForBinningDetailsByOpenGrinNo(openGrinNumber);
                        openGrinForBinningDetails.IsOpenGrinForBinningCompleted = true;
                        await _openGrinForBinningRepository.UpdateOpenGrinForBinning(openGrinForBinningDetails);

                    }


                    // Inventory Update Code
                    string grinPartId = "";

                    if (openGrinForBinningItemsDto != null)
                    {

                        var OpenGrinForGrinItemId = openGrinForBinningItemsDto.OpenGrinForGrinItemId;

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = openGrinForBinningItemsDto.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterObjectResult = await client.SendAsync(request);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK) getItemMas = itemMasterObjectResult.StatusCode;
                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        var itemMasterObjectData = JsonConvert.DeserializeObject<OpenGrinForBinningItemMasterDetails>(itemMasterObjectString);
                        var itemMasterObject = itemMasterObjectData.data;

                        var openGrinForBinningLocations = openGrinForBinningItemsDto.OpenGrinForBinningLocations;
                        int j = 0;
                        int k = 0;
                        foreach (var location in openGrinForBinningLocations)
                        {
                            if (j == 0)
                            {

                                var client1 = _clientFactory.CreateClient();
                                var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                var itemNo = openGrinForBinningItemsDto.ItemNumber;
                                var encodedItemNo = Uri.EscapeDataString(itemNo);
                                var openGrinNo = openGrinForBinningDetail.OpenGrinNumber;
                                var encodedopenGrinNo = Uri.EscapeDataString(openGrinNo);
                                var openGrinForGrinItemIds = openGrinForBinningItemsDto.OpenGrinForGrinItemId;
                                var referenceSONumbers = location.ReferenceSONumber;
                                var encodedReferenceSONumbers = Uri.EscapeDataString(referenceSONumbers);

                                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                    $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedopenGrinNo}&GrinPartsId={openGrinForGrinItemIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedReferenceSONumbers}"));
                                request1.Headers.Add("Authorization", token1);

                                var inventoryObjectResult = await client1.SendAsync(request1);
                                if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;
                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                dynamic inventoryObject = inventoryObjectData.data;

                                inventoryObject.balance_Quantity = location.Qty;
                                inventoryObject.warehouse = location.Warehouse;
                                inventoryObject.location = location.Location;
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                //var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                                //    "UpdateInventory?id=", inventoryObject.id), data);

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

                                //InventoryTranction Update Code
                                IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                                iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                                iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                                iqcInventoryTranctionDto.Description = inventoryObject.description;
                                iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                                iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                                iqcInventoryTranctionDto.Warehouse = inventoryObject.warehouse;
                                iqcInventoryTranctionDto.From_Location = inventoryObject.location;
                                iqcInventoryTranctionDto.TO_Location = inventoryObject.location;
                                iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; ;
                                iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                                iqcInventoryTranctionDto.ReferenceID = inventoryObject.referenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                iqcInventoryTranctionDto.ReferenceIDFrom = inventoryObject.referenceIDFrom;
                                iqcInventoryTranctionDto.GrinMaterialType = "OPGGRIN";
                                iqcInventoryTranctionDto.ShopOrderNo = "";

                                var jsonss = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                                var datass = new StringContent(jsonss, Encoding.UTF8, "application/json");
                                //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);
                                var client10 = _clientFactory.CreateClient();
                                var token10 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request10 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = datass
                                };
                                request10.Headers.Add("Authorization", token10);

                                var responses1 = await client10.SendAsync(request10);

                                if (responses1.StatusCode != HttpStatusCode.OK) createInvTrans = responses1.StatusCode;
                                j++;
                            }
                            else
                            {

                                var openGrinForGrinItemsDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(OpenGrinForGrinItemId);
                                BinningInventoryDtoPost inventoryObjectNew = new BinningInventoryDtoPost();
                                inventoryObjectNew.PartNumber = openGrinForBinningItemsDto.ItemNumber;
                                inventoryObjectNew.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); 
                                inventoryObjectNew.Description = itemMasterObject.description;
                                inventoryObjectNew.ProjectNumber = location.ReferenceSONumber;
                                inventoryObjectNew.Balance_Quantity = location.Qty;
                                inventoryObjectNew.Max = itemMasterObject.max;
                                inventoryObjectNew.Min = itemMasterObject.min;
                                inventoryObjectNew.UOM = openGrinForGrinItemsDetails.UOM;
                                inventoryObjectNew.Warehouse = location.Warehouse;
                                inventoryObjectNew.Location = location.Location;
                                inventoryObjectNew.GrinNo = openGrinForBinningDetail.OpenGrinNumber;
                                inventoryObjectNew.GrinPartId = OpenGrinForGrinItemId;
                                inventoryObjectNew.PartType = openGrinForGrinItemsDetails.ItemType; // we have to take parttype from grinparts model;
                                inventoryObjectNew.ReferenceID = openGrinForBinningDetail.OpenGrinNumber;
                                inventoryObjectNew.ReferenceIDFrom = "OPGGRIN";
                                inventoryObjectNew.LotNumber = openGrinForGrinItemsDetails.LotNumber;

                                var json = JsonConvert.SerializeObject(inventoryObjectNew);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");

                                //HttpResponseMessage response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);

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

                                //InventoryTranction Update Code
                                
                                BinningInventoryTranctionDto inventoryTranctionObjectNew = new BinningInventoryTranctionDto();
                                inventoryTranctionObjectNew.PartNumber = inventoryObjectNew.PartNumber;
                                inventoryTranctionObjectNew.MftrPartNumber = inventoryObjectNew.MftrPartNumber;
                                inventoryTranctionObjectNew.Description = inventoryObjectNew.Description;
                                inventoryTranctionObjectNew.ProjectNumber = inventoryObjectNew.ProjectNumber;
                                inventoryTranctionObjectNew.Issued_Quantity = inventoryObjectNew.Balance_Quantity;
                                inventoryTranctionObjectNew.UOM = inventoryObjectNew.UOM;
                                inventoryTranctionObjectNew.Warehouse = inventoryObjectNew.Warehouse;
                                inventoryTranctionObjectNew.From_Location = "OPGNIQC";
                                inventoryTranctionObjectNew.TO_Location = inventoryObjectNew.Location;
                                inventoryTranctionObjectNew.GrinNo = inventoryObjectNew.GrinNo;
                                inventoryTranctionObjectNew.GrinPartId = inventoryObjectNew.GrinPartId;
                                inventoryTranctionObjectNew.PartType = inventoryObjectNew.PartType; // we have to take parttype from grinparts model;
                                inventoryTranctionObjectNew.ReferenceID = inventoryObjectNew.ReferenceID;
                                inventoryTranctionObjectNew.ReferenceIDFrom = inventoryObjectNew.ReferenceIDFrom;

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
                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && createInv == HttpStatusCode.OK && getItemMas == HttpStatusCode.OK
                                                                                                                                && createInvTrans == HttpStatusCode.OK)
                    {
                        _openGrinForIQCRepository.SaveAsync();
                        _openGrinForBinningRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create CreateOpenGrinForBinningItems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenGrinForBinningItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    //Updating Binning Status in BinningItem

                    openGrinForBinningItems.IsOpenGrinForBinningCompleted = true;
                    openGrinForBinningDetail.OpenGrinForBinningItems = new List<OpenGrinForBinningItems> { openGrinForBinningItems };
                    await _openGrinForBinningRepository.CreateOpenGrinForBinning(openGrinForBinningDetail);
                    _openGrinForBinningRepository.SaveAsync();

                    //Updating Binning Status in OpenGrinForGrinItems

                    var openGrinForGrinItemsId = openGrinForBinningItemsDto.OpenGrinForGrinItemId;
                    var openGrinForGrinItemDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemsId);
                    openGrinForGrinItemDetails.IsOpenGrinForBinningCompleted = true;
                    await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemDetails);
                    _openGrinForGrinItemRepository.SaveAsync();

                    //Updating Binning Status in OpenGrinForIQCItems

                    var openGrinForIQCItemDetails = await _openGrinForIQCItemRepository.GetOpenGrinForIQCItemsDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemsId);
                    openGrinForIQCItemDetails.IsOpenGrinForBinningCompleted = true;
                    await _openGrinForIQCItemRepository.UpdateOpenGrinForIQCItems(openGrinForIQCItemDetails);
                    _openGrinForIQCItemRepository.SaveAsync();


                    //Updating Binning Status in OpenGrinForGrin

                    var openGrinForGrinPartsBinningStatuscount = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemsBinningStatusCount(openGrinForGrinItemDetails.OpenGrinForGrinId);

                    if (openGrinForGrinPartsBinningStatuscount == 0)
                    {
                        var openGrinForGrinDetails = await _openGrinForGrinRepository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                        openGrinForGrinDetails.IsOpenGrinForBinningCompleted = true;
                        await _openGrinForGrinRepository.UpdateOpenGrinForGrin(openGrinForGrinDetails);
                        _openGrinForGrinRepository.SaveAsync();

                    }

                    //Updating Binning Status in IQC & Binning

                    var openGrinForGrinBinningStatuscount = await _openGrinForGrinRepository.GetOpenGrinForGrinbinningStatusCount(openGrinNumber);

                    if (openGrinForGrinBinningStatuscount > 0)
                    {
                        var openGrinForIQCDetails = await _openGrinForIQCRepository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);
                        openGrinForIQCDetails.IsOpenGrinForBinningCompleted = true;
                        await _openGrinForIQCRepository.UpdateOpenGrinForIQC(openGrinForIQCDetails);


                        var openGrinForBinningDetails = await _openGrinForBinningRepository.GetOpenGrinForBinningDetailsByOpenGrinNo(openGrinNumber);
                        openGrinForBinningDetails.IsOpenGrinForBinningCompleted = true;
                        await _openGrinForBinningRepository.UpdateOpenGrinForBinning(openGrinForBinningDetails);

                    }

                    // Inventory Update Code
                    string grinPartId = "";

                    if (openGrinForBinningItemsDto != null)
                    {

                        var OpenGrinForGrinItemId = openGrinForBinningItemsDto.OpenGrinForGrinItemId;

                        //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                        //    "GetItemMasterByItemNumber?", "&ItemNumber=", openGrinForBinningItemsDto.ItemNumber));

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = openGrinForBinningItemsDto.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterObjectResult = await client.SendAsync(request);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK) getItemMas = itemMasterObjectResult.StatusCode;
                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        var itemMasterObjectData = JsonConvert.DeserializeObject<OpenGrinForBinningItemMasterDetails>(itemMasterObjectString);
                        var itemMasterObject = itemMasterObjectData.data;

                        var openGrinForBinningLocations = openGrinForBinningItemsDto.OpenGrinForBinningLocations;
                        int j = 0;
                        int k = 0;
                        foreach (var location in openGrinForBinningLocations)
                        {


                            if (j == 0)
                            {

                                var client1 = _clientFactory.CreateClient();
                                var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                var itemNo = openGrinForBinningItemsDto.ItemNumber;
                                var encodedItemNo = Uri.EscapeDataString(itemNo);
                                var openGrinNo = openGrinForBinningDetail.OpenGrinNumber;
                                var encodedopenGrinNo = Uri.EscapeDataString(openGrinNo);
                                var openGrinForGrinItemId = openGrinForBinningItemsDto.OpenGrinForGrinItemId;
                                var referenceSONumbers = location.ReferenceSONumber;
                                var encodedReferenceSONumbers = Uri.EscapeDataString(referenceSONumbers);

                                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                    $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedopenGrinNo}&GrinPartsId={openGrinForGrinItemId}&ItemNumber={encodedItemNo}&ProjectNumber={encodedReferenceSONumbers}"));
                                request1.Headers.Add("Authorization", token1);

                                var inventoryObjectResult = await client1.SendAsync(request1);
                                if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;
                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                dynamic inventoryObject = inventoryObjectData.data;

                                inventoryObject.balance_Quantity = location.Qty;
                                inventoryObject.warehouse = location.Warehouse;
                                inventoryObject.location = location.Location;
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                //var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                                //    "UpdateInventory?id=", inventoryObject.id), data);

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

                                //InventoryTranction Update Code
                                IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                                iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                                iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                                iqcInventoryTranctionDto.Description = inventoryObject.description;
                                iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                                iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                                iqcInventoryTranctionDto.Warehouse = inventoryObject.warehouse;
                                iqcInventoryTranctionDto.From_Location = inventoryObject.location;
                                iqcInventoryTranctionDto.TO_Location = inventoryObject.location;
                                iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; ;
                                iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                                iqcInventoryTranctionDto.ReferenceID = inventoryObject.referenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                iqcInventoryTranctionDto.ReferenceIDFrom = inventoryObject.referenceIDFrom;
                                iqcInventoryTranctionDto.GrinMaterialType = "OPGGRIN";
                                iqcInventoryTranctionDto.ShopOrderNo = "";

                                var jsonss = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                                var datass = new StringContent(jsonss, Encoding.UTF8, "application/json");
                                //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);
                                var client10 = _clientFactory.CreateClient();
                                var token10 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request10 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = datass
                                };
                                request10.Headers.Add("Authorization", token10);

                                var responses1 = await client10.SendAsync(request10);

                                if (responses1.StatusCode != HttpStatusCode.OK) createInvTrans = responses1.StatusCode;

                                j++;
                            }
                            else
                            {
                                BinningInventoryDtoPost inventoryObjectNew = new BinningInventoryDtoPost();
                                inventoryObjectNew.PartNumber = openGrinForBinningItemsDto.ItemNumber;
                                inventoryObjectNew.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); 
                                inventoryObjectNew.Description = itemMasterObject.description;
                                inventoryObjectNew.ProjectNumber = location.ReferenceSONumber;
                                inventoryObjectNew.Balance_Quantity = location.Qty;
                                inventoryObjectNew.Max = itemMasterObject.max;
                                inventoryObjectNew.Min = itemMasterObject.min;
                                inventoryObjectNew.UOM = openGrinForGrinItemDetails.UOM;
                                inventoryObjectNew.Warehouse = location.Warehouse;
                                inventoryObjectNew.Location = location.Location;
                                inventoryObjectNew.GrinNo = openGrinForBinningDetail.OpenGrinNumber;
                                inventoryObjectNew.GrinPartId = OpenGrinForGrinItemId;
                                inventoryObjectNew.PartType = openGrinForGrinItemDetails.ItemType; // we have to take parttype from grinparts model;
                                inventoryObjectNew.ReferenceID = openGrinForBinningDetail.OpenGrinNumber;
                                inventoryObjectNew.ReferenceIDFrom = "OPGGRIN";
                                inventoryObjectNew.LotNumber = openGrinForGrinItemDetails.LotNumber;

                                var json = JsonConvert.SerializeObject(inventoryObjectNew);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");

                                //HttpResponseMessage response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);

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

                                //InventoryTranction Update Code

                                BinningInventoryTranctionDto inventoryTranctionObjectNew = new BinningInventoryTranctionDto();
                                inventoryTranctionObjectNew.PartNumber = inventoryObjectNew.PartNumber;
                                inventoryTranctionObjectNew.MftrPartNumber = inventoryObjectNew.MftrPartNumber;
                                inventoryTranctionObjectNew.Description = inventoryObjectNew.Description;
                                inventoryTranctionObjectNew.ProjectNumber = inventoryObjectNew.ProjectNumber;
                                inventoryTranctionObjectNew.Issued_Quantity = inventoryObjectNew.Balance_Quantity;
                                inventoryTranctionObjectNew.UOM = inventoryObjectNew.UOM;
                                inventoryTranctionObjectNew.Warehouse = inventoryObjectNew.Warehouse;
                                inventoryTranctionObjectNew.From_Location = "OPGNIQC";
                                inventoryTranctionObjectNew.TO_Location = inventoryObjectNew.Location;
                                inventoryTranctionObjectNew.GrinNo = inventoryObjectNew.GrinNo;
                                inventoryTranctionObjectNew.GrinPartId = inventoryObjectNew.GrinPartId;
                                inventoryTranctionObjectNew.PartType = inventoryObjectNew.PartType; // we have to take parttype from grinparts model;
                                inventoryTranctionObjectNew.ReferenceID = inventoryObjectNew.ReferenceID;
                                inventoryTranctionObjectNew.ReferenceIDFrom = inventoryObjectNew.ReferenceIDFrom;

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
                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && createInv == HttpStatusCode.OK && getItemMas == HttpStatusCode.OK
                                                                                                                                 && createInvTrans == HttpStatusCode.OK)
                    {
                        _openGrinForIQCRepository.SaveAsync();
                        _openGrinForBinningRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside CreateOpenGrinForBinningItems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenGrinForBinning and OpenGrinForBinningItems Created Successfully";
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
    }
}
