using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Repository;
using static Tips.Grin.Api.Entities.DTOs.OpenGrinForGrinReportWithParamForTransDto;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OpenGrinForGrinController : ControllerBase
    {
        private IOpenGrinForGrinRepository _repository;
        private IOpenGrinForGrinItemRepository _openGrinForGrinItemRepository;
        private IOpenGrinForIQCRepository _openGrinForIQCRepository;
        private IOpenGrinForIQCItemRepository _openGrinForIQCItemRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly String _createdBy;
        private readonly String _unitname;

        public OpenGrinForGrinController(IOpenGrinForIQCItemRepository openGrinForIQCItemRepository, IOpenGrinForIQCRepository openGrinForIQCRepository, IHttpClientFactory clientFactory, IOpenGrinForGrinRepository repository, IHttpContextAccessor httpContextAccessor,
                                                IOpenGrinForGrinItemRepository openGrinForGrinItemRepository, IWebHostEnvironment webHostEnvironment, ILoggerManager logger, 
                                                IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _repository = repository;
            _openGrinForGrinItemRepository = openGrinForGrinItemRepository;
            _openGrinForIQCRepository = openGrinForIQCRepository;
            _openGrinForIQCItemRepository = openGrinForIQCItemRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenGrinForGrin([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<OpenGrinForGrinDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinForGrinDto>>();

            try
            {
                var openGrinForGrinDetials = await _repository.GetAllOpenGrinForGrin(pagingParameter, searchParams);

                if (openGrinForGrinDetials == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrinForGrin data not found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenGrinForGrin data not found in db");
                    return NotFound(serviceResponse);
                }
                var metadata = new
                {
                    openGrinForGrinDetials.TotalCount,
                    openGrinForGrinDetials.PageSize,
                    openGrinForGrinDetials.CurrentPage,
                    openGrinForGrinDetials.HasNext,
                    openGrinForGrinDetials.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all Grins");
                var result = _mapper.Map<IEnumerable<OpenGrinForGrinDto>>(openGrinForGrinDetials);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrinForGrins Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error {ex.Message}{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetOpenGrinForGrinSPReportWithParam([FromBody] OpenGrinReportWithParamDto openGrinReportWithParamDto)
        {
            ServiceResponse<IEnumerable<OpenGrinForGrinSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinForGrinSPReport>>();
            try
            {
                var products = await _repository.GetOpenGrinForGrinSPReportWithParam(openGrinReportWithParamDto.OpenGrinNumber, openGrinReportWithParamDto.SenderName,
                                                                                                                                       openGrinReportWithParamDto.ReceiptRefNo);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrinForGrin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenGrinForGrin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenGrinForGrin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetOpenGrinForGrinSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetOpenGrinForGrinSPReportWithParamForTrans([FromBody] OpenGrinForGrinReportWithParamForTransDto openGrinReportWithParamDto)
        {
            ServiceResponse<IEnumerable<OpenGrinForGrinSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinForGrinSPReport>>();
            try
            {
                var products = await _repository.GetOpenGrinForGrinSPReportWithParamForTrans(openGrinReportWithParamDto.OpenGrinNumber, openGrinReportWithParamDto.SenderName,
                                                                                                  openGrinReportWithParamDto.ReceiptRefNo,openGrinReportWithParamDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrinForGrin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenGrinForGrin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenGrinForGrin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetOpenGrinForGrinSPReportWithParamForTrans action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetOpenGrinForGrinSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<OpenGrinForGrinSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinForGrinSPReport>>();
            try
            {
                var products = await _repository.GetOpenGrinForGrinSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrinForGrin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenGrinForGrin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenGrinForGrin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetOpenGrinForGrinSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpenGrinForGrinDetailsbyId(int id)
        {
            ServiceResponse<OpenGrinForGrinDto> serviceResponse = new ServiceResponse<OpenGrinForGrinDto>();

            try
            {
                var openGrinForGrinDetailsById = await _repository.GetOpenGrinForGrinDetailsbyId(id);
                if (openGrinForGrinDetailsById == null)
                {
                    _logger.LogError($"OpenGrinForGrin details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrinForGrin details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned OpenGrinForGrin with id: {id}");

                    OpenGrinForGrinDto openGrinForGrinDto = _mapper.Map<OpenGrinForGrinDto>(openGrinForGrinDetailsById);
                    List<OpenGrinForGrinItemsDto> OpenGrinForGrinItemsList = new List<OpenGrinForGrinItemsDto>();

                    if (openGrinForGrinDetailsById.OpenGrinForGrinItems != null)
                    {
                        foreach (var openGrinForGrinDetails in openGrinForGrinDetailsById.OpenGrinForGrinItems)
                        {
                            OpenGrinForGrinItemsDto openGrinForGrinItemsDtos = _mapper.Map<OpenGrinForGrinItemsDto>(openGrinForGrinDetails);
                            openGrinForGrinItemsDtos.OGNProjectNumberDto = _mapper.Map<List<OpenGrinForGrinProjectNumberDto>>(openGrinForGrinDetails.OGNProjectNumber);
                            OpenGrinForGrinItemsList.Add(openGrinForGrinItemsDtos);
                        }
                    }

                    openGrinForGrinDto.OpenGrinForGrinItems = OpenGrinForGrinItemsList;
                    serviceResponse.Data = openGrinForGrinDto;
                    serviceResponse.Message = $"Returned openGrinForGrinById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside OpenGrinForGrinById action: {ex.Message}");
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

        [HttpPost]
        public async Task<IActionResult> CreateOpenGrinForGrin([FromBody] OpenGrinForGrinPostDto openGrinForGrinPostDto)
        {
            ServiceResponse<OpenGrinForGrinDto> serviceResponse = new ServiceResponse<OpenGrinForGrinDto>();

            try
            {
                string serverKey = GetServerKey();

                if (openGrinForGrinPostDto is null)
                {
                    _logger.LogError("OpenGrinForGrin object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenGrinForGrin object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid OpenGrinForGrin object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OpenGrinForGrin object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var openGrinForGrins = _mapper.Map<OpenGrinForGrin>(openGrinForGrinPostDto);
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));
                HttpStatusCode invStatusCode = HttpStatusCode.OK;

                if (serverKey == "avision")
                {
                    var openGrinNumber = await _repository.GenerateOpenGrinForGrinNumberForAvision();
                    openGrinForGrins.OpenGrinNumber = openGrinNumber;
                }
                else
                {
                    var dateFormat = days + months + years;
                    var openGrinNumber = await _repository.GenerateOpenGrinForGrinNumber();
                    openGrinForGrins.OpenGrinNumber = dateFormat + openGrinNumber;
                }
                var openGrinForGrinItemsDto = openGrinForGrinPostDto.OpenGrinForGrinItems;
                
                List<OpenGrinForGrinItems> openGrinForGrinItemsList = new List<OpenGrinForGrinItems>();

                foreach (var openGrinForGrinItem in openGrinForGrinItemsDto)
                {
                    OpenGrinForGrinItems grinParts = _mapper.Map<OpenGrinForGrinItems>(openGrinForGrinItem);
                    grinParts.OGNProjectNumber = _mapper.Map<List<OpenGrinForGrinProjectNumber>>(openGrinForGrinItem.OGNProjectNumberDto);
                    openGrinForGrinItemsList.Add(grinParts);
                }

                openGrinForGrins.OpenGrinForGrinItems = openGrinForGrinItemsList;
                openGrinForGrins.IsOpenGrinForGrinCompleted = true;

                await _repository.CreateOpenGrinForGrin(openGrinForGrins);
                _repository.SaveAsync();

                if (openGrinForGrins.OpenGrinForGrinItems != null)
                {
                    foreach (var openGrinForGrinItem in openGrinForGrins.OpenGrinForGrinItems)
                    {
                        var openGrinForGrinItemDetail = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItem.Id);
                        openGrinForGrinItemDetail.LotNumber = openGrinForGrins.OpenGrinNumber + openGrinForGrinItemDetail.Id;
                        await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemDetail);

                    }
                }

                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode createInvTrans = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;
                HttpStatusCode getItemForIqcResp = HttpStatusCode.OK;

                if (openGrinForGrinItemsList != null)
                {
                    foreach (var openGrinForGrinItem in openGrinForGrinItemsList)
                    {
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = openGrinForGrinItem.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterObjectResult = await client.SendAsync(request);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                            getItemmResp = itemMasterObjectResult.StatusCode;

                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        var itemMasterObjectData = JsonConvert.DeserializeObject<OpenGrinInvDetails>(itemMasterObjectString);
                        var itemMasterObject = itemMasterObjectData.data;
                        if (itemMasterObject.itemmasterAlternate.Count() > 0)
                        {
                            if (openGrinForGrinItem.OGNProjectNumber != null)
                            {
                                foreach (var project in openGrinForGrinItem.OGNProjectNumber)
                                {
                                    OGInventoryDtoPost inventory = new OGInventoryDtoPost();

                                    inventory.PartNumber = openGrinForGrinItem.ItemNumber;
                                    inventory.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                                    inventory.Description = openGrinForGrinItem.Description;
                                    inventory.ProjectNumber = project.ReferenceSONumber;
                                    inventory.Balance_Quantity = openGrinForGrinItem.Qty;
                                    inventory.IsStockAvailable = true;
                                    inventory.Max = itemMasterObject.max;
                                    inventory.Min = itemMasterObject.min;
                                    inventory.UOM = openGrinForGrinItem.UOM;
                                    inventory.Warehouse = "OPGGRIN";
                                    inventory.Location = "OPGGRIN";
                                    inventory.GrinNo = openGrinForGrins.OpenGrinNumber;
                                    inventory.GrinPartId = openGrinForGrinItem.Id;
                                    inventory.PartType = openGrinForGrinItem.ItemType; // we have to take parttype from grinparts model;
                                    inventory.GrinMaterialType = "";
                                    inventory.ReferenceID = Convert.ToString(openGrinForGrinItem.Id);
                                    inventory.ReferenceIDFrom = "OpenGrinForGrin";
                                    inventory.ShopOrderNo = "";
                                    inventory.Unit = "";
                                    inventory.LotNumber = openGrinForGrinItem.LotNumber;


                                    var json = JsonConvert.SerializeObject(inventory);
                                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                                    //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventoryFromGrin"), data);

                                    var client1 = _clientFactory.CreateClient();
                                    var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                    var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                    "CreateInventoryFromGrin"))
                                    {
                                        Content = data
                                    };
                                    request1.Headers.Add("Authorization", token1);

                                    var response = await client1.SendAsync(request1);
                                    if (response.StatusCode != HttpStatusCode.OK) createInv = response.StatusCode;


                                    OGInventoryTranctionDto inventoryTranction = new OGInventoryTranctionDto();

                                    inventoryTranction.PartNumber = openGrinForGrinItem.ItemNumber;
                                    inventoryTranction.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                                    inventoryTranction.Description = openGrinForGrinItem.Description;
                                    inventoryTranction.ProjectNumber = project.ReferenceSONumber;
                                    inventoryTranction.Issued_Quantity = openGrinForGrinItem.Qty;
                                    inventoryTranction.Issued_By = _createdBy;
                                    inventoryTranction.Issued_DateTime = DateTime.Now;
                                    inventoryTranction.IsStockAvailable = true;
                                    inventoryTranction.UOM = openGrinForGrinItem.UOM;
                                    inventoryTranction.Warehouse = "OPGGRIN";
                                    inventoryTranction.From_Location = "OPGGRIN";
                                    inventoryTranction.TO_Location = "OPGGRIN";
                                    inventoryTranction.GrinNo = openGrinForGrins.OpenGrinNumber;
                                    inventoryTranction.GrinPartId = openGrinForGrinItem.Id;
                                    inventoryTranction.PartType = openGrinForGrinItem.ItemType; // we have to take parttype from grinparts model;
                                    inventoryTranction.GrinMaterialType = "OpenGrinForGrin";
                                    inventoryTranction.ReferenceID = Convert.ToString(openGrinForGrinItem.Id);
                                    inventoryTranction.ReferenceIDFrom = "OpenGrinForGrin";
                                    inventoryTranction.shopOrderNo = "";
                                    inventoryTranction.Remarks = "OpenGrinForGrin Done";


                                    var jsons = JsonConvert.SerializeObject(inventoryTranction);
                                    var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                                    // var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);

                                    var client2 = _clientFactory.CreateClient();
                                    var token2 = HttpContext.Request.Headers["Authorization"].ToString();

                                    var request2 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                    "CreateInventoryTranction"))
                                    {
                                        Content = datas
                                    };
                                    request2.Headers.Add("Authorization", token2);

                                    var responses = await client1.SendAsync(request2);
                                    if (responses.StatusCode != HttpStatusCode.OK) createInvTrans = responses.StatusCode;
                                }
                            }
                        }
                        else
                        {
                            _logger.LogError($"Something went wrong inside Create OpenGrin action: ItemMasterAlternates is not available for the ItemNumber Other Service Calling");
                            serviceResponse.Data = null;
                            serviceResponse.Message = "Saving Failed";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                            return StatusCode(500, serviceResponse);
                        }
                    }
                }


                if (createInv == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                {
                    _repository.SaveAsync();

                    if (openGrinForGrins.OpenGrinForGrinItems != null)
                    {
                        List<string> openGrinForGrinItemItemNoList = new List<string>();

                        foreach (var openGrinForGrinItem in openGrinForGrins.OpenGrinForGrinItems)
                        {
                            var grinPartsProjectNoDtoDetail = _mapper.Map<string>(openGrinForGrinItem.ItemNumber);
                            openGrinForGrinItemItemNoList.Add(grinPartsProjectNoDtoDetail);
                        }

                        var jsonss = JsonConvert.SerializeObject(openGrinForGrinItemItemNoList);
                        var data4 = new StringContent(jsonss, Encoding.UTF8, "application/json");
                        var client5 = _clientFactory.CreateClient();
                        var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                        var request5 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterEnggAPI"],
                        "GetAllClosedIqcItemMasterItemNoList"))
                        {
                            Content = data4
                        };
                        request5.Headers.Add("Authorization", token5);

                        var itemMasterObjectResult = await client5.SendAsync(request5);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        {
                            getItemForIqcResp = itemMasterObjectResult.StatusCode;
                        }
                        if (itemMasterObjectResult.StatusCode == HttpStatusCode.OK)
                        {
                            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                            dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                            dynamic itemMasterObject = itemMasterObjectData;

                            if (itemMasterObject != null && itemMasterObject.Count > 0)
                            {
                                for (int i = 0; i < openGrinForGrins.OpenGrinForGrinItems.Count; i++)
                                {
                                    var grinPartItemNo = openGrinForGrins.OpenGrinForGrinItems[i].ItemNumber;
                                    for (int j = 0; j < itemMasterObject.Count; j++)
                                    {
                                        if (itemMasterObject[j] == grinPartItemNo)
                                        {
                                            var openGrinForIQCGrinDetails = _mapper.Map<OpenGrinForIQCConfirmationSaveDto>(openGrinForGrins);
                                            openGrinForIQCGrinDetails.OpenGrinForGrinId = openGrinForGrins.Id;
                                            openGrinForIQCGrinDetails.OpenGrinNumber = openGrinForGrins.OpenGrinNumber;

                                            OpenGrinForIQCConfirmationItemsSaveDto grinIQCConfirmationItemsSaveDto = _mapper.Map<OpenGrinForIQCConfirmationItemsSaveDto>(openGrinForGrins.OpenGrinForGrinItems[i]);
                                            grinIQCConfirmationItemsSaveDto.ItemNumber = openGrinForGrins.OpenGrinForGrinItems[i].ItemNumber;
                                            grinIQCConfirmationItemsSaveDto.OpenGrinForGrinItemId = openGrinForGrins.OpenGrinForGrinItems[i].Id;
                                            grinIQCConfirmationItemsSaveDto.ReceivedQty = openGrinForGrins.OpenGrinForGrinItems[i].Qty;
                                            grinIQCConfirmationItemsSaveDto.AcceptedQty = openGrinForGrins.OpenGrinForGrinItems[i].Qty;
                                            grinIQCConfirmationItemsSaveDto.RejectedQty = 0;
                                            openGrinForIQCGrinDetails.OpenGrinForIQCConfirmationItemsSaveDto = grinIQCConfirmationItemsSaveDto;

                                            await CreateOpenGrinForIQCItems(openGrinForIQCGrinDetails);

                                        }
                                    }
                                }
                            }

                        }

                    }

                }
                else
                {
                    _logger.LogError($"Something went wrong inside Create CreateOpenGrinForGrin action: Other Service Calling");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Saving Failed";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                serviceResponse.Data = null;
                serviceResponse.Message = "CreateOpenGrinForGrin Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetGrinById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOpenGrinForGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        private async Task<IActionResult> CreateOpenGrinForIQCItems([FromBody] OpenGrinForIQCConfirmationSaveDto openGrinForIQCSaveDto)
        {
            ServiceResponse<OpenGrinForIQCConfirmationSaveDto> serviceResponse = new ServiceResponse<OpenGrinForIQCConfirmationSaveDto>();

            try
            {
                string serverKey = GetServerKey();

                if (openGrinForIQCSaveDto is null)
                {
                    _logger.LogError("Create OpenGrinForIQCItems object sent from the client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Create OpenGrinForIQCItems object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Create OpenGrinForIQCItems object sent from the client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Create OpenGrinForIQCItems object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var openGrinForIQC = _mapper.Map<OpenGrinForIQC>(openGrinForIQCSaveDto);
                var openGrinForIQCItemsDto = openGrinForIQCSaveDto.OpenGrinForIQCConfirmationItemsSaveDto;
                var openGrinForIQCItems = _mapper.Map<OpenGrinForIQCItems>(openGrinForIQCItemsDto);
                var openGrinNumber = openGrinForIQC.OpenGrinNumber;
                var openGrinForGrinId = openGrinForIQC.OpenGrinForGrinId;

                HttpStatusCode getInvGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode getInvTrancGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInvTranc = HttpStatusCode.OK;
                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode createInvTrans = HttpStatusCode.OK;
                HttpStatusCode createInvTrans1 = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;

                var existingOpenGrinForIQCDetails = await _openGrinForIQCRepository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);

                if (existingOpenGrinForIQCDetails != null)
                {
                    openGrinForIQCItems.OpenGrinForIQCId = existingOpenGrinForIQCDetails.Id;

                    var openGrinForGrinItemId = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                    var openGrinForGrinItemDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);
                    if (openGrinForIQCItemsDto.OpenGrinForGrinItemId != openGrinForGrinItemDetails.Id)
                    {
                        _logger.LogError($"Invalid OpenGrinForGrin Part Id {openGrinForGrinItemId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid OpenGrinForGrin Part Id {openGrinForGrinItemId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }
                   

                    //Updating IQC Status in OpenGrinForIQCItems

                    openGrinForIQCItems.IsOpenGrinForIqcCompleted = true;
                    await _openGrinForIQCItemRepository.CreateOpenGrinForIQCItems(openGrinForIQCItems);
                    _openGrinForIQCItemRepository.SaveAsync();

                    ////update accepted qty, rejected qty and IQC Status in OpenGrinForGrinItems model

                    var updatedOpenGrinForGrinItemsQty = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItemsQty
                                                                       (openGrinForIQCItems.OpenGrinForGrinItemId, openGrinForIQCItems.AcceptedQty.ToString(),
                                                                       openGrinForIQCItems.RejectedQty.ToString());

                    var openGrinForGrinItems = _mapper.Map<OpenGrinForGrinItems>(updatedOpenGrinForGrinItemsQty);
                    openGrinForGrinItems.IsOpenGrinForIqcCompleted = true;
                    await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItems);
                    _openGrinForGrinItemRepository.SaveAsync();

                    //Updating IQC Status in OpenGrinForGrin And OpenGrinForIqc MainLevel

                    var openGrinForGrinItemsCount = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemsCount(openGrinForGrinId);
                    var openGrinForIQCItemsCount = await _openGrinForIQCItemRepository.GetOpenGrinForIQCItemsCount(existingOpenGrinForIQCDetails.Id);

                    if (openGrinForGrinItemsCount == openGrinForIQCItemsCount)
                    {
                        var openGrinForIQCDetails = await _openGrinForIQCRepository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);
                        openGrinForIQCDetails.IsOpenGrinForIqcCompleted = true;
                        await _openGrinForIQCRepository.UpdateOpenGrinForIQC(openGrinForIQCDetails);

                        var openGrinForGrinDetails = await _repository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                        openGrinForGrinDetails.IsOpenGrinForIqcCompleted = true;
                        await _repository.UpdateOpenGrinForGrin(openGrinForGrinDetails);
                    }

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = openGrinForIQCItemsDto.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        getItemmResp = itemMasterObjectResult.StatusCode;

                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;
                    //Inventory Update Code
                    decimal acceptedQty = openGrinForIQCItemsDto.AcceptedQty;
                    decimal rejectedQty = openGrinForIQCItemsDto.RejectedQty;
                    var grinPartsId = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                    var openGrinForGrinItemsDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);

                    foreach (var projectNo in openGrinForGrinItemsDetails.OGNProjectNumber)
                    {

                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = openGrinForIQCItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var openGrinForGrinNo = openGrinForIQC.OpenGrinNumber;
                        var encodedOpenGrinForGrinNo = Uri.EscapeDataString(openGrinForGrinNo);
                        var openGrinForGrinItemIds = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                        var referenceSONumber = projectNo.ReferenceSONumber;
                        var encodedreferenceSONumber = Uri.EscapeDataString(referenceSONumber);


                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedOpenGrinForGrinNo}&GrinPartsId={openGrinForGrinItemIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedreferenceSONumber}"));
                        request1.Headers.Add("Authorization", token1);

                        var inventoryObjectResult = await client1.SendAsync(request1);
                        if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;
                        var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic inventoryObject = inventoryObjectData.data;
                        if (inventoryObject != null)
                        {
                            decimal balanceQty = inventoryObject.balance_Quantity;
                            int flag1 = 0;
                            int flag2 = 0;
                            decimal bal = 0;
                            if (inventoryObject.balance_Quantity <= acceptedQty && inventoryObject.balance_Quantity != 0)
                            {
                                inventoryObject.warehouse = "OPGIQC";
                                inventoryObject.location = "OPGIQC";
                                inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                inventoryObject.referenceIDFrom = "OPGIQC";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "OPGIQC";
                                    inventoryObject.location = "OPGIQC";
                                    inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                    inventoryObject.referenceIDFrom = "OPGIQC";
                                    flag1 = 1;
                                }
                                else
                                {
                                    bal = inventoryObject.balance_Quantity - acceptedQty;
                                    if (bal != 0)
                                    {
                                        flag2 = 1;
                                    }
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "OPGIQC";
                                    inventoryObject.location = "OPGIQC";
                                    inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                    inventoryObject.referenceIDFrom = "OPGIQC";
                                    acceptedQty = 0;
                                }
                            }

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
                            iqcInventoryTranctionDto.IsStockAvailable = true;
                            iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto.Issued_By = _createdBy;
                            iqcInventoryTranctionDto.Issued_DateTime = DateTime.Now;
                            iqcInventoryTranctionDto.Warehouse = "OPGIQC";
                            iqcInventoryTranctionDto.From_Location = "OPGGRIN";
                            iqcInventoryTranctionDto.TO_Location = "OPGIQC";
                            iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; 
                            iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.referenceID;
                            iqcInventoryTranctionDto.ReferenceIDFrom = inventoryObject.referenceIDFrom;
                            iqcInventoryTranctionDto.GrinMaterialType = "OPGGRIN";
                            iqcInventoryTranctionDto.shopOrderNo = "";
                            iqcInventoryTranctionDto.Remarks = "OpenGrinForGrinIqc Done";

                            string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            var contents = new StringContent(rfqSourcingPPdetailsJsons, Encoding.UTF8, "application/json");
                            var client7 = _clientFactory.CreateClient();
                            var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = contents
                            };
                            request7.Headers.Add("Authorization", token7);

                            var inventoryTransResponses = await client7.SendAsync(request7);

                            if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTrans1 = inventoryTransResponses.StatusCode;

                            if (openGrinForIQCItemsDto.RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = openGrinForIQCItems.ItemNumber;
                                grinInventoryDto.LotNumber = openGrinForGrinItemDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = itemMasterObject.mftrItemNumber;
                                grinInventoryDto.Description = itemMasterObject.itemDescription;
                                grinInventoryDto.ProjectNumber =referenceSONumber;
                                //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(openGrinForIQCItems.RejectedQty);
                                grinInventoryDto.Max = itemMasterObject.max;
                                grinInventoryDto.Min = itemMasterObject.min;
                                grinInventoryDto.UOM = openGrinForGrinItemDetails.UOM;
                                grinInventoryDto.Warehouse = "Reject";
                                grinInventoryDto.Location = "Reject";
                                grinInventoryDto.GrinNo = openGrinForIQC.OpenGrinNumber;
                                grinInventoryDto.GrinPartId = openGrinForIQCItems.OpenGrinForGrinItemId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;
                                grinInventoryDto.ReferenceID = openGrinForIQC.OpenGrinForIQCNumber; // Convert.ToString(openGrinForIQCItems.Id) //;
                                grinInventoryDto.ReferenceIDFrom = "OPGIQC";
                                grinInventoryDto.GrinMaterialType = "OPGGRIN";
                                grinInventoryDto.ShopOrderNo = "";

                                if (flag1 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = rejectedQty;

                                }
                                else if (flag2 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = bal;
                                    rejectedQty -= bal;

                                }

                                string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                var client6 = _clientFactory.CreateClient();
                                var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                "CreateInventoryFromGrin"))
                                {
                                    Content = content
                                };
                                request6.Headers.Add("Authorization", token6);

                                var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInv = rfqCustomerIdResponse.StatusCode;

                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.IsStockAvailable = false;
                                iqcInventoryTranctionDtos.Issued_By = _createdBy;
                                iqcInventoryTranctionDtos.Issued_DateTime = DateTime.Now;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = "OPGGRIN";
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo; ;
                                iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.ReferenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "OPGIQC";
                                iqcInventoryTranctionDtos.GrinMaterialType = "OPGGRIN";
                                iqcInventoryTranctionDtos.shopOrderNo = "";
                                iqcInventoryTranctionDtos.Remarks = "OpenGrinForGrinIqc Done";

                                string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                var client8 = _clientFactory.CreateClient();
                                var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents1
                                };
                                request8.Headers.Add("Authorization", token8);

                                var inventoryTransResponses1 = await client8.SendAsync(request8);

                                if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTrans = inventoryTransResponses1.StatusCode;
                            }

                        }
                    }

                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && getInvTrancGrinId == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTrans1 == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                    {
                        _openGrinForIQCRepository.SaveAsync();
                        _repository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create CreateOpenGrinForIQCItems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                    serviceResponse.Data = null;
                    serviceResponse.Message = "openGrinForIQCItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (serverKey == "avision")
                    {
                        var openGrinNum = openGrinForIQC.OpenGrinNumber;
                        if (openGrinNum != null)
                        {
                            var openGrinForIqcNum = openGrinNum.Replace("OPGN", "OPGNIQC");
                            openGrinForIQC.OpenGrinForIQCNumber = openGrinForIqcNum;
                        }

                    }

                    var openGrinForGrinItemId = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                    var openGrinForGrinItemDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);
                    if (openGrinForIQCItemsDto.OpenGrinForGrinItemId != openGrinForGrinItemDetails.Id)
                    {
                        _logger.LogError($"Invalid OpenGrinForGrinItem Part Id {openGrinForGrinItemId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid OpenGrinForGrinItem Part Id {openGrinForGrinItemId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    //Updating IQC Status in OpenGrinForIQCItems
                    openGrinForIQCItems.IsOpenGrinForIqcCompleted = true;
                    openGrinForIQCItems.OpenGrinForIQCId = openGrinForIQC.Id;
                    openGrinForIQC.OpenGrinForIQCItems = new List<OpenGrinForIQCItems> { openGrinForIQCItems };
                    await _openGrinForIQCRepository.CreateOpenGrinForIQC(openGrinForIQC);
                    _openGrinForIQCRepository.SaveAsync();

                    ////update accepted qty, rejected qty and IQC Status in OpenGrinForGrinItems 

                    var updatedOpenGrinForGrinItemsQty = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItemsQty
                                                                        (openGrinForIQCItems.OpenGrinForGrinItemId, openGrinForIQCItems.AcceptedQty.ToString(),
                                                                        openGrinForIQCItems.RejectedQty.ToString());

                    var openGrinForGrinItems = _mapper.Map<OpenGrinForGrinItems>(updatedOpenGrinForGrinItemsQty);
                    openGrinForGrinItems.IsOpenGrinForIqcCompleted = true;
                    string result = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItems);
                    _openGrinForGrinItemRepository.SaveAsync();

                    //Updating IQC Status in OpenGrinForGrin And OpenGrinForIqc MainLevel

                    var openGrinForGrinItemsCount = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemsCount(openGrinForGrinId);
                    var openGrinForIQCItemsCount = await _openGrinForIQCItemRepository.GetOpenGrinForIQCItemsCount(openGrinForIQC.Id);

                    if (openGrinForGrinItemsCount == openGrinForIQCItemsCount)
                    {
                        var openGrinForIQCDetails = await _openGrinForIQCRepository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);
                        openGrinForIQCDetails.IsOpenGrinForIqcCompleted = true;
                        await _openGrinForIQCRepository.UpdateOpenGrinForIQC(openGrinForIQCDetails);

                        var openGrinForGrinDetails = await _repository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                        openGrinForGrinDetails.IsOpenGrinForIqcCompleted = true;
                        await _repository.UpdateOpenGrinForGrin(openGrinForGrinDetails);
                    }

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = openGrinForIQCItemsDto.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        getItemmResp = itemMasterObjectResult.StatusCode;

                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;

                    //Inventory Update Code
                    decimal acceptedQty = openGrinForIQCItemsDto.AcceptedQty;
                    decimal rejectedQty = openGrinForIQCItemsDto.RejectedQty;
                    var openGrinForGrinPartsId = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                    var openGrinForGrinItemDetail = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);
                    foreach (var projectNo in openGrinForGrinItemDetail.OGNProjectNumber)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = openGrinForIQCItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var openGrinForGrinNo = openGrinForIQC.OpenGrinNumber;
                        var encodedOpenGrinForGrinNo = Uri.EscapeDataString(openGrinForGrinNo);
                        var openGrinForGrinItemIds = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                        var referenceSONumber = projectNo.ReferenceSONumber;
                        var encodedreferenceSONumber = Uri.EscapeDataString(referenceSONumber);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedOpenGrinForGrinNo}&GrinPartsId={openGrinForGrinItemIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedreferenceSONumber}"));
                        request1.Headers.Add("Authorization", token1);

                        var inventoryObjectResult = await client1.SendAsync(request1);
                        if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;
                        var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic inventoryObject = inventoryObjectData.data;
                        if (inventoryObject != null)
                        {
                            decimal balanceQty = inventoryObject.balance_Quantity;
                            int flag1 = 0;
                            int flag2 = 0;
                            decimal bal = 0;

                            if (inventoryObject.balance_Quantity <= acceptedQty && inventoryObject.balance_Quantity != 0)
                            {
                                inventoryObject.warehouse = "OPGIQC";
                                inventoryObject.location = "OPGIQC";
                                inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                inventoryObject.referenceIDFrom = "OPGIQC";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "OPGIQC";
                                    inventoryObject.location = "OPGIQC";
                                    inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                    inventoryObject.referenceIDFrom = "OPGIQC";
                                    inventoryObject.isStockAvailable = false;
                                    flag1 = 1;
                                }
                                else
                                {
                                    bal = inventoryObject.balance_Quantity - acceptedQty;
                                    if (bal != 0)
                                    {
                                        flag2 = 1;
                                    }
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "OPGIQC";
                                    inventoryObject.location = "OPGIQC";
                                    inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                    inventoryObject.referenceIDFrom = "OPGIQC";
                                    acceptedQty = 0;
                                }
                            }

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
                            iqcInventoryTranctionDto.IsStockAvailable = true;
                            iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto.Issued_By = _createdBy;
                            iqcInventoryTranctionDto.Issued_DateTime = DateTime.Now;
                            iqcInventoryTranctionDto.Warehouse = "OPGIQC";
                            iqcInventoryTranctionDto.From_Location = "OPGGRIN";
                            iqcInventoryTranctionDto.TO_Location = "OPGIQC";
                            iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; 
                            iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.referenceID;
                            iqcInventoryTranctionDto.ReferenceIDFrom = inventoryObject.referenceIDFrom;
                            iqcInventoryTranctionDto.GrinMaterialType = "OPGGRIN";
                            iqcInventoryTranctionDto.shopOrderNo = "";
                            iqcInventoryTranctionDto.Remarks = "OpenGrinForGrinIqc Done";

                            string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            var contents = new StringContent(rfqSourcingPPdetailsJsons, Encoding.UTF8, "application/json");
                            var client7 = _clientFactory.CreateClient();
                            var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = contents
                            };
                            request7.Headers.Add("Authorization", token7);

                            var inventoryTransResponses = await client7.SendAsync(request7);

                            if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTrans1 = inventoryTransResponses.StatusCode;

                            if (openGrinForIQCItemsDto.RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = openGrinForIQCItems.ItemNumber;
                                grinInventoryDto.LotNumber = openGrinForGrinItemDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = itemMasterObject.mftrItemNumber;
                                grinInventoryDto.Description = itemMasterObject.itemDescription;
                                grinInventoryDto.ProjectNumber = referenceSONumber;
                                //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(openGrinForIQCItems.RejectedQty);
                                grinInventoryDto.Max = itemMasterObject.max;
                                grinInventoryDto.Min = itemMasterObject.min;
                                grinInventoryDto.UOM = openGrinForGrinItemDetails.UOM;
                                grinInventoryDto.Warehouse = "Reject";
                                grinInventoryDto.Location = "Reject";
                                grinInventoryDto.GrinNo = openGrinForIQC.OpenGrinNumber;
                                grinInventoryDto.GrinPartId = openGrinForIQCItems.OpenGrinForGrinItemId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;
                                grinInventoryDto.ReferenceID = openGrinForIQC.OpenGrinForIQCNumber; // Convert.ToString(openGrinForIQCItems.Id) //;
                                grinInventoryDto.ReferenceIDFrom = "OPGIQC";
                                grinInventoryDto.GrinMaterialType = "OPGGRIN";
                                grinInventoryDto.ShopOrderNo = "";

                                if (flag1 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = rejectedQty;

                                }
                                else if (flag2 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = bal;
                                    rejectedQty -= bal;

                                }

                                string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                var client6 = _clientFactory.CreateClient();
                                var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                "CreateInventoryFromGrin"))
                                {
                                    Content = content
                                };
                                request6.Headers.Add("Authorization", token6);

                                var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInv = rfqCustomerIdResponse.StatusCode;

                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.IsStockAvailable = false;
                                iqcInventoryTranctionDtos.Issued_By = _createdBy;
                                iqcInventoryTranctionDtos.Issued_DateTime = DateTime.Now;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = "OPGGRIN";
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo; 
                                iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.ReferenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "OPGIQC";
                                iqcInventoryTranctionDtos.GrinMaterialType = "OPGGRIN";
                                iqcInventoryTranctionDtos.shopOrderNo = "";
                                iqcInventoryTranctionDtos.Remarks = "OpenGrinForGrinIqc Done";

                                string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                var client8 = _clientFactory.CreateClient();
                                var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents1
                                };
                                request8.Headers.Add("Authorization", token8);

                                var inventoryTransResponses1 = await client8.SendAsync(request8);

                                if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTrans = inventoryTransResponses1.StatusCode;
                            }



                        }
                    }

                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && getInvTrancGrinId == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTrans1 == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                    {
                        _openGrinForIQCRepository.SaveAsync();
                        _repository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create CreateOpenGrinForIQCItems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = null;
                    serviceResponse.Message = "openGrinForIQC and openGrinForIQCItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateIQCConfirmationItems action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpenGrinForGrin(int id, [FromBody] OpenGrinForGrinUpdateDto openGrinForGrinUpdateDto)
        {
            ServiceResponse<OpenGrinForGrinDto> serviceResponse = new ServiceResponse<OpenGrinForGrinDto>();

            try
            {
                if (openGrinForGrinUpdateDto is null)
                {
                    _logger.LogError("Update OpenGrinForGrin object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update OpenGrinForGrin object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update OpenGrinForGrin object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update OpenGrinForGrin object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateOpenGrinForGrin = await _repository.GetOpenGrinForGrinDetailsbyId(id);
                if (updateOpenGrinForGrin is null)
                {
                    _logger.LogError($"Update Grin with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var openGrinForGrinDetail = _mapper.Map<OpenGrinForGrin>(openGrinForGrinUpdateDto);

                var openGrinForGrinItemsDto = updateOpenGrinForGrin.OpenGrinForGrinItems;
                var openGrinForGrinItemList = new List<OpenGrinForGrinItems>();

                foreach (var openGrinForGrinItem in openGrinForGrinItemsDto)
                {
                    OpenGrinForGrinItems openGrinForGrinItems = _mapper.Map<OpenGrinForGrinItems>(openGrinForGrinItem);
                    openGrinForGrinItems.OGNProjectNumber = _mapper.Map<List<OpenGrinForGrinProjectNumber>>(openGrinForGrinItem.OGNProjectNumber);
                    openGrinForGrinItemList.Add(openGrinForGrinItems);
                }
                var data = _mapper.Map(openGrinForGrinUpdateDto, updateOpenGrinForGrin);
                data.OpenGrinForGrinItems = openGrinForGrinItemList;

                string result = await _repository.UpdateOpenGrinForGrin(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OpenGrinForGrin Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateOpenGrinForGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenGrinNumberForOpenGrinIqc()
        {
            ServiceResponse<IEnumerable<OpenGrinNoForOpenGrinIqcAndBinning>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinNoForOpenGrinIqcAndBinning>>();
            try
            {
                var openGrinNumberForOpenGrinIqc = await _repository.GetAllOpenGrinNumberForOpenGrinIqc();
                var result = _mapper.Map<IEnumerable<OpenGrinNoForOpenGrinIqcAndBinning>>(openGrinNumberForOpenGrinIqc);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrinNumberForOpenGrinIqc";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllOpenGrinNumberForOpenGrinIqc action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenGrinNumberForOpenGrinBinning()
        {
            ServiceResponse<IEnumerable<OpenGrinNoForOpenGrinIqcAndBinning>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinNoForOpenGrinIqcAndBinning>>();
            try
            {
                var openGrinNumberForOpenGrinBinning = await _repository.GetAllOpenGrinNumberForOpenGrinBinning();
                var result = _mapper.Map<IEnumerable<OpenGrinNoForOpenGrinIqcAndBinning>>(openGrinNumberForOpenGrinBinning);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrinNumberForOpenGrinBinning";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllOpenGrinNumberForOpenGrinBinning action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
