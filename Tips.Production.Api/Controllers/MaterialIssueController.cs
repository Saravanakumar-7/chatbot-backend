using AutoMapper;
using AutoMapper.Execution;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;
using Tips.Production.Api.Repository;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class MaterialIssueController : ControllerBase
    {
        private IMaterialIssueHistoryRepository _materialIssueHistoryRepository;
        private IMaterialIssueRepository _materialIssueRepository;
        private IMaterialIssueItemRepository _materialIssueItemRepository;
        private IShopOrderRepository _shopOrderRepository;
        private ILoggerManager _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private IMapper _mapper;
        private readonly IHttpClientFactory _clientFactory;

        public MaterialIssueController(IMaterialIssueItemRepository materialIssueItemRepository, IHttpClientFactory clientFactory,
            IMaterialIssueHistoryRepository materialIssueHistoryRepository, IMaterialIssueRepository materialIssueRepository,
            ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config, IShopOrderRepository shopOrderRepository)
        {
            _materialIssueItemRepository = materialIssueItemRepository;
            _materialIssueHistoryRepository = materialIssueHistoryRepository;
            _materialIssueRepository = materialIssueRepository;
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
            _mapper = mapper;
            _shopOrderRepository = shopOrderRepository;
            _clientFactory = clientFactory;
        }

        // GET: api/<MaterialIssueController>
        [HttpGet]
        public async Task<IActionResult> GetAllMaterialIssues([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<MaterialIssueDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueDto>>();
            try
            {
                var materialIssueDetails = await _materialIssueRepository.GetAllMaterialIssues(pagingParameter, searchParamess);
                var metadata = new
                {
                    materialIssueDetails.TotalCount,
                    materialIssueDetails.PageSize,
                    materialIssueDetails.CurrentPage,
                    materialIssueDetails.HasNext,
                    materialIssueDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all MaterialIssues");
                var result = _mapper.Map<IEnumerable<MaterialIssueDto>>(materialIssueDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialIssue";
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
        public async Task<IActionResult> MaterialIssueSPReport()
        {
            var products = await _materialIssueRepository.MaterialIssueSPReport();

            return Ok(products);
        }
        [HttpGet]
        public async Task<IActionResult> GetMaterialIssueSPReportForTrans([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<MaterialIssueSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueSPReportForTrans>>();
            try
            {
                var products = await _materialIssueRepository.GetMaterialIssueSPReportForTrans(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all GetMaterialIssueSPReport");

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssue hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialIssue hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MaterialIssueSPReportForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetMaterialIssueSPReportForTrans action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetMaterialIssueSPReportWithParam([FromBody] MaterialIssueReportWithParamDto materialIssueReportWithParamDto)
        {
            ServiceResponse<IEnumerable<MaterialIssueSPReport>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueSPReport>>();
            try
            {
                var products = await _materialIssueRepository.GetMaterialIssueSPReportWithParam(materialIssueReportWithParamDto.ShopOrderNumber,
                                                                            materialIssueReportWithParamDto.FGitemnumber, materialIssueReportWithParamDto.ProjectNumber,
                                                                            materialIssueReportWithParamDto.SalesOrderNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssue hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialIssue hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MaterialIssue Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetMaterialIssueSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetMaterialIssueSPReportWithParamForTrans([FromBody] MaterialIssueReportWithParamDtoForTrans materialIssueReportWithParamDto)
        {
            ServiceResponse<IEnumerable<MaterialIssueSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueSPReportForTrans>>();
            try
            {
                var products = await _materialIssueRepository.GetMaterialIssueSPReportWithParamForTrans(materialIssueReportWithParamDto.WorkorderNo,
                                                                            materialIssueReportWithParamDto.ItemNumber, materialIssueReportWithParamDto.ProjectNumber,
                                                                            materialIssueReportWithParamDto.SalesOrderNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssue hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialIssue hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MaterialIssueSPReportWithParamForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetMaterialIssueSPReportWithParamForTrans action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetMaterialIssueSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<MaterialIssueSPReport>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueSPReport>>();
            try
            {
                var products = await _materialIssueRepository.GetMaterialIssueSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssue hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialIssue hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MaterialIssue Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetMaterialIssueSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetMaterialIssueSPReportWithDateForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<MaterialIssueSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueSPReportForTrans>>();
            try
            {
                var products = await _materialIssueRepository.GetMaterialIssueSPReportWithDateForTrans(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssue hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialIssue hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MaterialIssueSPReportWithDateForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetMaterialIssueSPReportWithDateForTrans action";
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
        //private string GetServerKey()
        //{
        //    var serverName = Dns.GetHostName();

        //    if (serverName == "Server1")
        //    {
        //        return "keus";
        //    }
        //    else if (serverName == "Server2")
        //    {
        //        return "avision";
        //    }
        //    else
        //    {
        //        return "trasccon";
        //    }
        //}


        // GET api/<MaterialIssueController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialIssueById(int id)
        {
            ServiceResponse<MaterialIssuesDto> serviceResponse = new ServiceResponse<MaterialIssuesDto>();
            try
            {
                string serverKey = GetServerKey();// Set the server key here dynamically based on your logic

                var materialIssueDetailById = await _materialIssueRepository.GetMaterialIssueById(id);
                var shoporder = await _shopOrderRepository.GetShopOrderDetailsByShopOrderNo(materialIssueDetailById.ShopOrderNumber);
                if (materialIssueDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssue with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialIssue with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var materialIssueDetails = _mapper.Map<MaterialIssuesDto>(materialIssueDetailById);
                    if (shoporder.ShopOrderConfirmationStatus== ShopOrderConformationStatus.FullyDone) materialIssueDetails.IsShopOrderconfirmed = true;
                    List<string> MaterialIssueItemProjectNumbers = await _materialIssueItemRepository.GetMaterialIssueItemProjectNumbersById(materialIssueDetailById.Id);
                    if (MaterialIssueItemProjectNumbers.Count > 0)
                    {
                        materialIssueDetails.ProjectNumber = string.Join(",", MaterialIssueItemProjectNumbers);
                    }
                    List<MaterialIssueItemsDto> materialIssueItemDtos = materialIssueDetails.materialIssueItems;

                    var groupedMaterialIssueItemDtoList = materialIssueItemDtos
                    .GroupBy(item => item.PartNumber)
                    .Select(group => new MaterialIssueItemsDto
                    {
                        Id = group.First().Id,
                        PartNumber = group.Key,
                        Description = group.First().Description,
                        ProjectNumber = group.First().ProjectNumber,
                        PartType = group.First().PartType,
                        UOM = group.First().UOM,
                        RequiredQty = group.Sum(item => item.RequiredQty),
                        AvailableQty = 0,     //group.First().AvailableQty,
                        IssuedQty = group.Sum(item => item.IssuedQty),
                        Unit = group.First().Unit,
                        CreatedBy = group.First().CreatedBy,
                        CreatedOn = group.First().CreatedOn,
                        LastModifiedBy = group.First().LastModifiedBy,
                        LastModifiedOn = group.First().LastModifiedOn,
                        MaterialIssuedStatus = group.First().MaterialIssuedStatus,
                        MaterialIssueId = group.First().MaterialIssueId,
                        MRNQty = group.First().MRNQty
                    })
            .ToList();

                    List<MaterialIssueItemsDto> groupedMaterialIssueItemList = new List<MaterialIssueItemsDto>();
                    for (int i = 0; i < groupedMaterialIssueItemDtoList.Count(); i++)
                    {
                        decimal balanceQuantity = 0;
                        //var partnumber = materialIssueDetailById.materialIssueItems[i].PartNumber;
                        //var projectnumber = materialIssueDetailById.materialIssueItems[i].ProjectNumber;
                        //var partnumber = Uri.EscapeDataString(groupedMaterialIssueItemDtoList[i].PartNumber);
                        //var projectnumber = Uri.EscapeDataString(groupedMaterialIssueItemDtoList[i].ProjectNumber);
                        //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                        //     "GetInventoryStockByItemAndProjectNo?", "itemNumber=", partnumber, "&projectNumber=", projectnumber));

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = groupedMaterialIssueItemDtoList[i].PartNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);
                        var projectNumber = groupedMaterialIssueItemDtoList[i].ProjectNumber;
                        var encodedProjectNumber = Uri.EscapeDataString(projectNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryStockByItemAndProjectNo?itemNumber={encodedItemNumber}&projectNumber={encodedProjectNumber}"));
                        request.Headers.Add("Authorization", token);

                        var inventoryObjectResult = await client.SendAsync(request);
                        if (inventoryObjectResult != null && inventoryObjectResult.StatusCode == HttpStatusCode.OK)
                        {
                            var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                            dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                            JArray inventoryArray = inventoryObjectData.data; // Use JArray instead of 
                            balanceQuantity = 0;

                            if (inventoryArray != null)
                            {
                                foreach (var item in inventoryArray)
                                {
                                    decimal itemBalanceQty = Convert.ToDecimal(item["balanceQty"]);
                                    balanceQuantity += itemBalanceQty;
                                }
                            }
                        }

                        groupedMaterialIssueItemDtoList[i].AvailableQty = balanceQuantity;

                        groupedMaterialIssueItemList.Add(groupedMaterialIssueItemDtoList[i]);

                    }

                    //} 
                    materialIssueDetails.materialIssueItems = groupedMaterialIssueItemList;
                    serviceResponse.Data = materialIssueDetails;
                    serviceResponse.Message = "Returned MaterialIssue with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMaterialIssueById action: {ex.Message} {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialIssueByShopOrderNo(string shopOrderNumber)
        {
            ServiceResponse<MaterialIssuesDto> serviceResponse = new ServiceResponse<MaterialIssuesDto>();

            try
            {
                var materialIssueDetail = await _materialIssueRepository.GetMaterialIssueByShopOrderNo(shopOrderNumber);

                if (materialIssueDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssue with shopOrderNumber hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"MaterialIssue with shopOrderNumber: {shopOrderNumber}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else

                {
                    _logger.LogInfo($"Returned MaterialIssueDetails with shopOrderNumber: {shopOrderNumber}");

                    //MaterialIssueDto materialIssueDto = _mapper.Map<MaterialIssueDto>(materialIssueDetail);

                    //List<MaterialIssueItemDto> MaterialIssueItemList = new List<MaterialIssueItemDto>();

                    //foreach (var materialIssueItemDetails in materialIssueDetail.materialIssueItems)
                    //{
                    //    MaterialIssueItemDto MaterialIssueItemDto = _mapper.Map<MaterialIssueItemDto>(materialIssueItemDetails);
                    //    MaterialIssueItemList.Add(MaterialIssueItemDto);
                    //}

                    //materialIssueDto.materialIssueItems = MaterialIssueItemList;
                    //serviceResponse.Data = materialIssueDto;
                    var materialIssueDetails = _mapper.Map<MaterialIssuesDto>(materialIssueDetail);
                    List<string> MaterialIssueItemProjectNumbers = await _materialIssueItemRepository.GetMaterialIssueItemProjectNumbersById(materialIssueDetail.Id);
                    if (MaterialIssueItemProjectNumbers.Count > 0)
                    {
                        materialIssueDetails.ProjectNumber = string.Join(",", MaterialIssueItemProjectNumbers);
                    }
                    List<MaterialIssueItemsDto> materialIssueItemDtos = materialIssueDetails.materialIssueItems;

                    var groupedMaterialIssueItemDtoList = materialIssueItemDtos
                    .GroupBy(item => item.PartNumber)
                    .Select(group => new MaterialIssueItemsDto
                    {
                        Id = group.First().Id,
                        PartNumber = group.Key,
                        Description = group.First().Description,
                        ProjectNumber = group.First().ProjectNumber,
                        PartType = group.First().PartType,
                        UOM = group.First().UOM,
                        RequiredQty = group.Sum(item => item.RequiredQty),
                        AvailableQty = 0,     //group.First().AvailableQty,
                        IssuedQty = group.Sum(item => item.IssuedQty),
                        Unit = group.First().Unit,
                        CreatedBy = group.First().CreatedBy,
                        CreatedOn = group.First().CreatedOn,
                        LastModifiedBy = group.First().LastModifiedBy,
                        LastModifiedOn = group.First().LastModifiedOn,
                        MaterialIssuedStatus = group.First().MaterialIssuedStatus,
                        MaterialIssueId = group.First().MaterialIssueId
                    })
            .ToList();

                    List<MaterialIssueItemsDto> groupedMaterialIssueItemList = new List<MaterialIssueItemsDto>();
                    for (int i = 0; i < groupedMaterialIssueItemDtoList.Count(); i++)
                    {
                        decimal balanceQuantity = 0;
                        //var partnumber = materialIssueDetailById.materialIssueItems[i].PartNumber;
                        //var projectnumber = materialIssueDetailById.materialIssueItems[i].ProjectNumber;
                        //var partnumber = Uri.EscapeDataString(groupedMaterialIssueItemDtoList[i].PartNumber);
                        //var projectnumber = Uri.EscapeDataString(groupedMaterialIssueItemDtoList[i].ProjectNumber);
                        //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                        //     "GetInventoryStockByItemAndProjectNo?", "itemNumber=", partnumber, "&projectNumber=", projectnumber));

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = groupedMaterialIssueItemDtoList[i].PartNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);
                        var projectNumber = groupedMaterialIssueItemDtoList[i].ProjectNumber;
                        var encodedProjectNumber = Uri.EscapeDataString(projectNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryStockByItemAndProjectNo?itemNumber={encodedItemNumber}&projectNumber={encodedProjectNumber}"));
                        request.Headers.Add("Authorization", token);

                        var inventoryObjectResult = await client.SendAsync(request);
                        if (inventoryObjectResult != null && inventoryObjectResult.StatusCode == HttpStatusCode.OK)
                        {
                            var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                            dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                            JArray inventoryArray = inventoryObjectData.data; // Use JArray instead of 
                            balanceQuantity = 0;

                            if (inventoryArray != null)
                            {
                                foreach (var item in inventoryArray)
                                {
                                    decimal itemBalanceQty = Convert.ToDecimal(item["balanceQty"]);
                                    balanceQuantity += itemBalanceQty;
                                }
                            }
                        }

                        groupedMaterialIssueItemDtoList[i].AvailableQty = balanceQuantity;

                        groupedMaterialIssueItemList.Add(groupedMaterialIssueItemDtoList[i]);

                    }

                    //} 
                    materialIssueDetails.materialIssueItems = groupedMaterialIssueItemList;
                    serviceResponse.Data = materialIssueDetails;
                    serviceResponse.Message = $"Returned MaterialIssueDetails";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMaterialIssueByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // POST api/<MaterialIssueController>
        [HttpPost]
        public IActionResult CreateMaterialIssue([FromBody] MaterialIssuePostDto materialIssuePostDto)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();

            try
            {
                if (materialIssuePostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialIssue object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("MaterialIssue object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialIssue object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid MaterialIssue object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var materialIssue = _mapper.Map<MaterialIssue>(materialIssuePostDto);

                _materialIssueRepository.CreateMaterialIssue(materialIssue);
                _materialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialIssue Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Created("GetMaterialIssueById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateMaterialIssue action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<MaterialIssueController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialIssue(int id, [FromBody] MaterialIssueUpdateDto materialIssueUpdateDto)
        {
            ServiceResponse<MaterialIssueUpdateDto> serviceResponse = new ServiceResponse<MaterialIssueUpdateDto>();

            try
            {
                if (materialIssueUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialIssue object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("MaterialIssue object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialIssue object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid MaterialIssue object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var materialIssueDetailsById = await _materialIssueRepository.GetMaterialIssueById(id);
                if (materialIssueDetailsById is null)
                {
                    _logger.LogError($"MaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update MaterialIssue with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var allMaterialIssueItems = await _materialIssueItemRepository.GetMaterialIssueItemById(id);
                //List<InventoryDtoForMaterialIssue> inventoryDtoForIssue = new List<InventoryDtoForMaterialIssue>();

                // get latest production bom version by passing fgnumber
                //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"], 
                //                        "GetLatestEnggProductionBomVersionDetailByItemNumber?","&fgPartNumber=", materialIssueDetailsById.ItemNumber));
                //var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                //dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                //dynamic itemMasterObject = itemMasterObjectData.data;

                if (allMaterialIssueItems == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Internal Server Error!";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Material Issue Item with Given MaterailId does not existins. Error in  UpdateMaterialIssue action");
                    return NotFound(serviceResponse);
                }

                ShopOrder shopOrderDetail = await _shopOrderRepository.GetShopOrderByShopOrderNo(materialIssueDetailsById.ShopOrderNumber);
                decimal bomRevNo = shopOrderDetail.BomRevisionNo;
                HttpStatusCode updateMaterialIssueResp = HttpStatusCode.OK;
                List<MaterialIssueItemUpdateDto> materialIssueItemDtos = materialIssueUpdateDto.MaterialIssueItems;
                foreach (var updatedItem in materialIssueItemDtos)
                {
                    if (updatedItem.NewIssueQty > 0)
                    {
                        var existingItem = allMaterialIssueItems
                            .FirstOrDefault(i => i.Id == updatedItem.Id);

                        if (existingItem != null)
                        {

                            existingItem.IssuedQty += updatedItem.NewIssueQty;

                            var projectNo = existingItem.ProjectNumber;
                            decimal newIssuedQty = updatedItem.NewIssueQty;
                            var partnumber = updatedItem.PartNumber;

                            //Add SO Material Issue tracker table

                            InventoryDtoForMaterialIssue inventoryDtoForIssue = new InventoryDtoForMaterialIssue();
                            inventoryDtoForIssue.PartNumber = partnumber;
                            inventoryDtoForIssue.ProjectNumber = projectNo;
                            inventoryDtoForIssue.DataFrom = "ShopOrder";
                            inventoryDtoForIssue.Bomversion = bomRevNo;
                            inventoryDtoForIssue.IssueQty = newIssuedQty;
                            inventoryDtoForIssue.ShopOrderNumber = materialIssueUpdateDto.ShopOrderNumber;
                            var json = JsonConvert.SerializeObject(inventoryDtoForIssue);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "UpdateInventoryOnMaterialIssue"), data);

                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                            "UpdateInventoryOnMaterialIssue"))
                            {
                                Content = data
                            };
                            request1.Headers.Add("Authorization", token1);

                            var response = await client1.SendAsync(request1);
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                updateMaterialIssueResp = response.StatusCode;
                            }

                            //InventoryTranctionForMaterialIssue inventoryTranction = new InventoryTranctionForMaterialIssue();
                            //inventoryTranction.PartNumber = partnumber;
                            //inventoryTranction.MftrPartNumber = partnumber;
                            //inventoryTranction.ProjectNumber = projectNo;
                            //inventoryTranction.Issued_Quantity = newIssuedQty;
                            //inventoryTranction.shopOrderNo = materialIssueUpdateDto.ShopOrderNumber;
                            //inventoryTranction.Issued_DateTime = DateTime.Now;
                            //inventoryTranction.ReferenceIDFrom = "MaterialIssue";
                            //inventoryTranction.Issued_By = materialIssueUpdateDto.CreatedBy;
                            //inventoryTranction.Remarks = "Update MaterialIssue";
                            //var jsons = JsonConvert.SerializeObject(inventoryTranction);
                            //var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                            //var results = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);



                            await _materialIssueItemRepository.UpdateMaterialIssueItem(existingItem);

                        }
                    }
                }
                if (updateMaterialIssueResp == HttpStatusCode.OK)
                {
                    _materialIssueItemRepository.SaveAsync();
                }
                else
                {
                    _logger.LogError($"Something went wrong inside UpdateMaterialIssue action. Inventory update action UpdateInventoryOnMaterialIssue failed! ");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Internal server error";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                //string result = await _materialIssueRepository.UpdateMaterialIssue(materialIssueDetailsById);

                //_logger.LogInfo(result);
                //_materialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialIssue Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateMaterialIssue action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> IssueMaterialIssue(int id, [FromBody] IssueMaterialIssueUpdateDto materialIssueUpdateDto)
        {
            ServiceResponse<MaterialIssueUpdateDto> serviceResponse = new ServiceResponse<MaterialIssueUpdateDto>();

            try
            {
                if (materialIssueUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialIssue object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("MaterialIssue object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialIssue object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid MaterialIssue object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var materialIssueDetailsById = await _materialIssueRepository.GetMaterialIssueById(id);
                if (materialIssueDetailsById is null)
                {
                    _logger.LogError($"MaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update MaterialIssue with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var allMaterialIssueItems = await _materialIssueItemRepository.GetMaterialIssueItemById(id);

                if (allMaterialIssueItems == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Internal Server Error!";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Material Issue Item with Given MaterailId does not existins. Error in  UpdateMaterialIssue action");
                    return NotFound(serviceResponse);
                }

                ShopOrder shopOrderDetail = await _shopOrderRepository.GetShopOrderByShopOrderNo(materialIssueDetailsById.ShopOrderNumber);
                decimal bomRevNo = shopOrderDetail.BomRevisionNo;
                HttpStatusCode updateMaterialIssueResp = HttpStatusCode.OK;
                List<IssueMaterialIssueItemUpdateDto> materialIssueItemDtos = materialIssueUpdateDto.MaterialIssueItems;

                foreach (var updatedItem in materialIssueItemDtos)
                {
                    if (updatedItem.NewIssueQty > 0)
                    {
                        var existingItem = allMaterialIssueItems
                            .FirstOrDefault(i => i.Id == updatedItem.Id);

                        if (existingItem != null)
                        {
                            List<MaterialIssueLocation> materialIssueLocationDetails = _mapper.Map<List<MaterialIssueLocation>>(updatedItem.MaterialIssueLocationDto.ToList());

                            existingItem.IssuedQty += updatedItem.NewIssueQty;
                            existingItem.MaterialIssueLocations = materialIssueLocationDetails;

                            var projectNo = existingItem.ProjectNumber;
                            decimal newIssuedQty = updatedItem.NewIssueQty;
                            var partnumber = updatedItem.PartNumber;

                            //Add SO Material Issue tracker table
                            foreach (var materialIssueLocation in updatedItem.MaterialIssueLocationDto)
                            {
                                InventoryDtoForMaterialIssueLocation inventoryDtoForIssue = new InventoryDtoForMaterialIssueLocation();
                                inventoryDtoForIssue.PartNumber = partnumber;
                                inventoryDtoForIssue.ProjectNumber = projectNo;
                                inventoryDtoForIssue.DataFrom = "ShopOrder";
                                inventoryDtoForIssue.Bomversion = bomRevNo;
                                inventoryDtoForIssue.ShopOrderNumber = materialIssueUpdateDto.ShopOrderNumber;
                                inventoryDtoForIssue.Warehouse = materialIssueLocation.Warehouse;
                                inventoryDtoForIssue.Location = materialIssueLocation.Location;
                                inventoryDtoForIssue.DistributingQty = materialIssueLocation.DistributingQty;
                                inventoryDtoForIssue.LotNumber = materialIssueLocation.LotNumber;

                                var json = JsonConvert.SerializeObject(inventoryDtoForIssue);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");

                                var client1 = _clientFactory.CreateClient();
                                var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                "UpdateInventoryOnMaterialIssueLocation"))
                                {
                                    Content = data
                                };
                                request1.Headers.Add("Authorization", token1);

                                var response = await client1.SendAsync(request1);
                                if (response.StatusCode != HttpStatusCode.OK)
                                {
                                    updateMaterialIssueResp = response.StatusCode;
                                }

                                await _materialIssueItemRepository.UpdateMaterialIssueItem(existingItem);


                            }
                        }
                    }
                }
                if (updateMaterialIssueResp == HttpStatusCode.OK)
                {
                    _materialIssueItemRepository.SaveAsync();
                }
                else
                {
                    _logger.LogError($"Something went wrong inside UpdateMaterialIssue action. Inventory update action UpdateInventoryOnMaterialIssue failed! ");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Internal server error";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialIssue Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateMaterialIssue action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchMaterialIssueDate([FromQuery] SearchDateparames searchDateParam)
        {
            ServiceResponse<IEnumerable<MaterialIssueReportDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueReportDto>>();
            try
            {
                var materialIssueDetails = await _materialIssueRepository.SearchMaterialIssueDate(searchDateParam);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialIssue, MaterialIssueReportDto>()
                       .ForMember(dest => dest.materialIssueItems, opt => opt.MapFrom(src => src.materialIssueItems
                       .Select(materialIssueItems => new MaterialIssueItemReportDto
                       {
                           Id = materialIssueItems.Id,
                           ShopOrderNumber = src.ShopOrderNumber,
                           PartNumber = materialIssueItems.PartNumber,
                           Description = materialIssueItems.Description,
                           ProjectNumber = materialIssueItems.ProjectNumber,
                           PartType = materialIssueItems.PartType,
                           UOM = materialIssueItems.UOM,
                           RequiredQty = materialIssueItems.RequiredQty,
                           //AvailableQty = materialIssue.AvailableQty,
                           IssuedQty = materialIssueItems.IssuedQty,
                           Unit = materialIssueItems.Unit,
                           MaterialIssuedStatus = materialIssueItems.MaterialIssuedStatus
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialIssueReportDto>>(materialIssueDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all materialIssueDetails";
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
        public async Task<IActionResult> GetAllMaterialIssueWithItems([FromBody] MaterialIssueSearchDto materialIssueSearch)
        {
            ServiceResponse<IEnumerable<MaterialIssueReportDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueReportDto>>();
            try
            {
                var materialIssueDetails = await _materialIssueRepository.GetAllMaterialIssueWithItems(materialIssueSearch);



                _logger.LogInfo("Returned all materialIssueDetails");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<MaterialIssueDto, MaterialIssue>().ReverseMap()
                //    .ForMember(dest => dest.materialIssueItems, opt => opt.MapFrom(src => src.materialIssueItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialIssue, MaterialIssueReportDto>()
                       .ForMember(dest => dest.materialIssueItems, opt => opt.MapFrom(src => src.materialIssueItems
                       .Select(materialIssueItems => new MaterialIssueItemReportDto
                       {
                           Id = materialIssueItems.Id,
                           ShopOrderNumber = src.ShopOrderNumber,
                           PartNumber = materialIssueItems.PartNumber,
                           Description = materialIssueItems.Description,
                           ProjectNumber = materialIssueItems.ProjectNumber,
                           PartType = materialIssueItems.PartType,
                           UOM = materialIssueItems.UOM,
                           RequiredQty = materialIssueItems.RequiredQty,
                           //AvailableQty = materialIssue.AvailableQty,
                           IssuedQty = materialIssueItems.IssuedQty,
                           Unit = materialIssueItems.Unit,
                           MaterialIssuedStatus = materialIssueItems.MaterialIssuedStatus
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialIssueReportDto>>(materialIssueDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialIssueDetails";
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
        public async Task<IActionResult> SearchMaterialIssue([FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<MaterialIssueReportDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueReportDto>>();
            try
            {
                var materialIssueDetails = await _materialIssueRepository.SearchMaterialIssue(searchParams);

                _logger.LogInfo("Returned all materialIssueDetails");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<MaterialIssueDto, MaterialIssue>().ReverseMap()
                //    .ForMember(dest => dest.materialIssueItems, opt => opt.MapFrom(src => src.materialIssueItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialIssue, MaterialIssueReportDto>()
                       .ForMember(dest => dest.materialIssueItems, opt => opt.MapFrom(src => src.materialIssueItems
                       .Select(materialIssueItems => new MaterialIssueItemReportDto
                       {
                           Id = materialIssueItems.Id,
                           ShopOrderNumber = src.ShopOrderNumber,
                           PartNumber = materialIssueItems.PartNumber,
                           Description = materialIssueItems.Description,
                           ProjectNumber = materialIssueItems.ProjectNumber,
                           PartType = materialIssueItems.PartType,
                           UOM = materialIssueItems.UOM,
                           RequiredQty = materialIssueItems.RequiredQty,
                           //AvailableQty = materialIssue.AvailableQty,
                           IssuedQty = materialIssueItems.IssuedQty,
                           Unit = materialIssueItems.Unit,
                           MaterialIssuedStatus = materialIssueItems.MaterialIssuedStatus
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialIssueReportDto>>(materialIssueDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialIssueDetails";
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

        // DELETE api/<MaterialIssueController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialIssue(int id)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();

            try
            {
                var materialIssueDetailById = await _materialIssueRepository.GetMaterialIssueById(id);
                if (materialIssueDetailById == null)
                {
                    _logger.LogError($"Delete MaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete MaterialIssue with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _materialIssueRepository.DeleteMaterialIssue(materialIssueDetailById);
                _logger.LogInfo(result);
                _materialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialIssue Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteMaterialIssue action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaterialIssueIdNameList()
        {
            ServiceResponse<IEnumerable<MaterialIssueIdNameList>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueIdNameList>>();
            try
            {
                var listOfAllMaterialIssueIdNames = await _materialIssueRepository.GetAllMaterialIssueIdNameList();
                var result = _mapper.Map<IEnumerable<MaterialIssueIdNameList>>(listOfAllMaterialIssueIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All listOfAllMaterialIssueIdNames";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllMaterialIssueIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        //[HttpPost] // Adjust your route as needed
        //public async Task<IActionResult> PickListProductionSPReport([FromQuery] PickListProductionDTO pickListProduction)

        //{
        //    ServiceResponse<IEnumerable<PickList>> serviceResponse = new ServiceResponse<IEnumerable<PickList>>();
        //    try
        //    {
        //        var products = await _materialIssueRepository.PickListProductionSPReport(pickListProduction?.ShopOrderNumber);

        //        if (products == null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"PickListProductionSPReport hasn't been found.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            _logger.LogError($"PickListProduction hasn't been found in db.");
        //            return NotFound(serviceResponse);
        //        }
        //        else
        //        {
        //            var result = _mapper.Map<IEnumerable<PickListProductionDTO>>(products);

        //            serviceResponse.Data = products;
        //            serviceResponse.Message = "Returned PickListProduction Details";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(serviceResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside PickListProduction action";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
    }
}
