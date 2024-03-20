using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShopOrderController : ControllerBase
    {
        private IShopOrderRepository _shopOrderRepository;
        private IShopOrderItemRepository _shopOrderItemRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IMaterialIssueRepository _materialIssueRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ShopOrderController(IShopOrderItemRepository shopOrderItemRepository, IShopOrderRepository shopOrderRepository, IHttpContextAccessor httpContextAccessor,
            IMaterialIssueRepository materialIssueRepository, ILoggerManager logger,
            IMapper mapper, IConfiguration config, HttpClient httpClient)
        {
            _logger = logger;
            _shopOrderRepository = shopOrderRepository;
            _mapper = mapper;
            _materialIssueRepository = materialIssueRepository;
            _shopOrderItemRepository = shopOrderItemRepository;
            _httpClient = httpClient;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShopOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<ShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderDto>>();

            try
            {
                var shopOrderDetails = await _shopOrderRepository.GetAllShopOrders(pagingParameter, searchParamess);
                var metadata = new
                {
                    shopOrderDetails.TotalCount,
                    shopOrderDetails.PageSize,
                    shopOrderDetails.CurrentPage,
                    shopOrderDetails.HasNext,
                    shopOrderDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ShopOrders()s");
                var result = _mapper.Map<IEnumerable<ShopOrderDto>>(shopOrderDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ShopOrders Successfully";
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

        [HttpGet]
        public async Task<IActionResult> ShopOrderNumberSPReport()
        {
            var products = await _shopOrderRepository.ShopOrderNumberSPReport();

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> GetShopOrderSPReportWithParam([FromBody] ShopOrderReportWithParamDto shopOrderReportWithParamDto)
        {
            ServiceResponse<IEnumerable<ShopOrderNumberSPReport>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderNumberSPReport>>();
            try
            {
                var products = await _shopOrderRepository.GetShopOrderSPReportWithParam(shopOrderReportWithParamDto.ShopOrderNumber,
                                                                            shopOrderReportWithParamDto.ProjectType, shopOrderReportWithParamDto.ProjectNumber,
                                                                            shopOrderReportWithParamDto.SalesOrderNumber, shopOrderReportWithParamDto.KPN,
                                                                            shopOrderReportWithParamDto.MPN);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ShopOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ShopOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetShopOrderSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetShopOrderSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<ShopOrderNumberSPReport>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderNumberSPReport>>();
            try
            {
                var products = await _shopOrderRepository.GetShopOrderSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ShopOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ShopOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetShopOrderSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchShopOrderDate([FromQuery] SearchDateparames searchDateParam)
        {
            ServiceResponse<IEnumerable<ShopOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderReportDto>>();
            try
            {
                var shopOrderDetails = await _shopOrderRepository.SearchShopOrderDate(searchDateParam);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<ShopOrder, ShopOrderReportDto>()
                       .ForMember(dest => dest.ShopOrderItems, opt => opt.MapFrom(src => src.ShopOrderItems
                       .Select(shopOrderItem => new ShopOrderItemReportDto
                       {
                           Id = shopOrderItem.Id,
                           ShopOrderNumber = src.ShopOrderNumber,
                           FGItemNumber = shopOrderItem.FGItemNumber,
                           Description = shopOrderItem.Description,
                           ProjectNumber = shopOrderItem.ProjectNumber,
                           SalesOrderNumber = shopOrderItem.SalesOrderNumber,
                           OpenSalesOrderQty = shopOrderItem.OpenSalesOrderQty,
                           ReleaseQty = shopOrderItem.ReleaseQty,
                           RequiredQty = shopOrderItem.RequiredQty,
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<ShopOrderReportDto>>(shopOrderDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all shopOrderDetails";
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
        public async Task<IActionResult> GetAllShopOrderWithItems([FromBody] ShopOrderSearchDto shopOrderSearch)
        {
            ServiceResponse<IEnumerable<ShopOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderReportDto>>();
            try
            {
                var ShopOrderDetails = await _shopOrderRepository.GetAllShopOrderWithItems(shopOrderSearch);

                _logger.LogInfo("Returned all ShopOrderDetails");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<ShopOrderDto, ShopOrder>().ReverseMap()
                //    .ForMember(dest => dest.ShopOrderItems, opt => opt.MapFrom(src => src.ShopOrderItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<ShopOrder, ShopOrderReportDto>()
                       .ForMember(dest => dest.ShopOrderItems, opt => opt.MapFrom(src => src.ShopOrderItems
                       .Select(shopOrderItem => new ShopOrderItemReportDto
                       {
                           Id = shopOrderItem.Id,
                           ShopOrderNumber = src.ShopOrderNumber,
                           FGItemNumber = shopOrderItem.FGItemNumber,
                           Description = shopOrderItem.Description,
                           ProjectNumber = shopOrderItem.ProjectNumber,
                           SalesOrderNumber = shopOrderItem.SalesOrderNumber,
                           OpenSalesOrderQty = shopOrderItem.OpenSalesOrderQty,
                           ReleaseQty = shopOrderItem.ReleaseQty,
                           RequiredQty = shopOrderItem.RequiredQty,
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<ShopOrderReportDto>>(ShopOrderDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ShopOrderDetails";
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
        public async Task<IActionResult> SearchShopOrder([FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<ShopOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderReportDto>>();
            try
            {
                var shopOrderDetails = await _shopOrderRepository.SearchShopOrder(searchParams);

                _logger.LogInfo("Returned all ShopOrderDetails");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<ShopOrderDto, ShopOrder>().ReverseMap()
                //    .ForMember(dest => dest.ShopOrderItems, opt => opt.MapFrom(src => src.ShopOrderItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<ShopOrder, ShopOrderReportDto>()
                       .ForMember(dest => dest.ShopOrderItems, opt => opt.MapFrom(src => src.ShopOrderItems
                       .Select(shopOrderItem => new ShopOrderItemReportDto
                       {
                           Id = shopOrderItem.Id,
                           ShopOrderNumber = src.ShopOrderNumber,
                           FGItemNumber = shopOrderItem.FGItemNumber,
                           Description = shopOrderItem.Description,
                           ProjectNumber = shopOrderItem.ProjectNumber,
                           SalesOrderNumber = shopOrderItem.SalesOrderNumber,
                           OpenSalesOrderQty = shopOrderItem.OpenSalesOrderQty,
                           ReleaseQty = shopOrderItem.ReleaseQty,
                           RequiredQty = shopOrderItem.RequiredQty,
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<ShopOrderReportDto>>(shopOrderDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ShopOrderDetails";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShopOrderById(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrderDetailById = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrderDetailById == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {id}");
                    ShopOrderDto shopOrderDto = _mapper.Map<ShopOrderDto>(shopOrderDetailById);

                    List<ShopOrderItemDto> shopOrderItemDtos = new List<ShopOrderItemDto>();
                    if (shopOrderDetailById.ShopOrderItems != null)
                    {
                        foreach (var shopOrderItemdetail in shopOrderDetailById.ShopOrderItems)
                        {
                            ShopOrderItemDto shopOrderItemDto = _mapper.Map<ShopOrderItemDto>(shopOrderItemdetail);
                            shopOrderItemDtos.Add(shopOrderItemDto);
                        }
                    }
                    shopOrderDto.ShopOrderItems = shopOrderItemDtos;
                    serviceResponse.Data = shopOrderDto;
                    serviceResponse.Message = "GetShopOrderById Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFGShopOrderNo()
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();

            try
            {
                var fGShopOrderNoList = await _shopOrderRepository.GetAllFGShopOrderNoList();
                _logger.LogInfo("Returned all FGShopOrderNo");

                var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(fGShopOrderNoList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FGShopOrderNo Successfully";
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

        [HttpGet]
        public async Task<IActionResult> GetAllSAShopOrderNo()
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();

            try
            {
                var fGShopOrderNoList = await _shopOrderRepository.GetAllSAShopOrderNoList();
                _logger.LogInfo("Returned all SAShopOrderNo");

                var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(fGShopOrderNoList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SAShopOrderNo Successfully";
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

        [HttpGet]
        public async Task<IActionResult> GetAllShopOrderNoListByProjectNo(string projectNo, PartType partType)
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();

            try
            {
                var shopOrderNoList = await _shopOrderRepository.GetAllActiveShopOrderNoListByProjectNo(projectNo, partType);
                _logger.LogInfo("Returned all ShopOrderNoList");

                var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(shopOrderNoList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ShopOrderNoList Successfully";
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

        [HttpPost]
        public async Task<IActionResult> CreateShopOrder([FromBody] ShopOrderPostDto shopOrderPostDto)
        {
            ServiceResponse<ShopOrderPostDto> serviceResponse = new ServiceResponse<ShopOrderPostDto>();

            try
            {
                string serverKey = GetServerKey();

                if (shopOrderPostDto == null)
                {
                    _logger.LogError("ShopOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShopOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShopOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ShopOrder object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var shopOrder = _mapper.Map<ShopOrder>(shopOrderPostDto);
                var shopOrderDto = shopOrderPostDto.ShopOrderItems;


                var ShoporderItemList = new List<ShopOrderItem>();
                if (shopOrderDto != null)
                {
                    for (int i = 0; i < shopOrderDto.Count; i++)
                    {
                        ShopOrderItem shopOrderItemDetail = _mapper.Map<ShopOrderItem>(shopOrderDto[i]);

                        //var salesObjectResult = await _httpClient.GetAsync(string.Concat(_config["SalesOrderAPI"],
                        //     "UpdateShopOrderQty?", "salesOrderNumber=", shopOrderDto[i].SalesOrderNumber,
                        //     "projectNumber=", shopOrderDto[i].ProjectNumber,"&itemNumber=",
                        //      shopOrderDto[i].FGItemNumber, "&releaseQty=", shopOrderDto[i].ReleaseQty));
                        if (shopOrder.ItemType == PartType.FG)
                        {
                            var shopOrderDetail = _mapper.Map<UpdateSalesOrderQtyDto>(shopOrderDto[i]);

                            var jsons = JsonConvert.SerializeObject(shopOrderDetail);
                            var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                            var responses = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "UpdateShopOrderQty?"), datas);
                            if(responses.StatusCode!=HttpStatusCode.OK)
                            {
                                _logger.LogError($"Something went wrong inside CreateShopOrder action");
                                serviceResponse.Data = null;
                                serviceResponse.Message = $"Something went wrong in sales Order update";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.NotFound;
                                return StatusCode(404, serviceResponse);
                            }
                        }
                        ShoporderItemList.Add(shopOrderItemDetail);

                    }
                }
                shopOrder.ShopOrderItems = ShoporderItemList;

                if (serverKey == "trasccon")
                {
                    var date = DateTime.Now;
                    var days = Convert.ToString(date.Day.ToString("D2"));
                    var months = Convert.ToString(date.Month.ToString("D2"));
                    var years = Convert.ToString(date.ToString("yy"));
                    var dateFormat = days + months + years;
                    var soNumber = await _shopOrderRepository.GenerateSONumber();
                    shopOrder.ShopOrderNumber = dateFormat + soNumber;
                }
                else if (serverKey == "avision")
                {
                    var soNumber = await _shopOrderRepository.GenerateSONumberForAvision();
                    shopOrder.ShopOrderNumber = soNumber;
                }
                else if (serverKey == "keus")
                {

                    var date = DateTime.Now;
                    var days = Convert.ToString(date.Day.ToString("D2"));
                    var months = Convert.ToString(date.Month.ToString("D2"));
                    var years = Convert.ToString(date.ToString("yy"));
                    var dateFormat = days + months + years;
                    var soNumber = await _shopOrderRepository.GenerateSONumberForKeus();
                    shopOrder.ShopOrderNumber = dateFormat + soNumber;
                }
                else
                {
                    Guid shopOrderNumber = Guid.NewGuid();
                    shopOrder.ShopOrderNumber = "SH-" + shopOrderNumber.ToString();
                }

                await _shopOrderRepository.CreateShopOrder(shopOrder);

                _shopOrderRepository.SaveAsync();


                // After Shop Order Creation Material Issue also should be created.
                await CreateMaterialIssueDetails(shopOrder);

                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetShopOrderItemById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateShopOrder action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"{ex.Message},{ex.InnerException}";
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
        private async Task<IActionResult> CreateMaterialIssueDetails(ShopOrder shopOrder)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();
            try
            {
                dynamic bomData = null;

                // Create MaterialIssue object outside the loop
                MaterialIssue materialIssue = new MaterialIssue();
                materialIssue.ShopOrderNumber = shopOrder.ShopOrderNumber;
                materialIssue.ShopOrderDate = shopOrder.CreatedOn;
                materialIssue.ProjectType = ProjectType.RFQ;
                materialIssue.ItemType = shopOrder.ItemType;
                materialIssue.ShopOrderQty = shopOrder.TotalSOReleaseQty;
                materialIssue.ItemNumber = shopOrder.ItemNumber;
                materialIssue.MaterialIssuedStatus = IssuedStatus.Open;
                materialIssue.IsShortClosed = false;
                materialIssue.BomRevisionNo = shopOrder.BomRevisionNo;
                List<MaterialIssueItem> materialIssueItemList = new List<MaterialIssueItem>();

                for (int i = 0; i < shopOrder.ShopOrderItems.Count(); i++)
                {
                    if (i == 0)
                    {
                        var fgNumber = shopOrder.ItemNumber;
                        decimal bomversion = shopOrder.BomRevisionNo;
                        var bomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"], "GetProductionBomByItemAndBomVersionNo?", "ItemNumber=", fgNumber, "&bomVersionNo=", bomversion));
                        var bomDetailsString = await bomDetails.Content.ReadAsStringAsync();
                        dynamic bomDetailsData = JsonConvert.DeserializeObject(bomDetailsString);
                        bomData = bomDetailsData.data;
                    }
                    if (shopOrder.ShopOrderItems[i].ReleaseQty <= 0)
                    {
                        continue;
                    }
                    if (bomData != null)
                    {
                        foreach (var bom in bomData.enggChildItemDtos)
                        {
                            var projectNo = shopOrder.ShopOrderItems[i].ProjectNumber;
                            MaterialIssueItem materialIssueItem = new MaterialIssueItem();
                            materialIssueItem.PartNumber = bom.itemNumber;
                            materialIssueItem.Description = bom.description;
                            materialIssueItem.ProjectNumber = projectNo;
                            materialIssueItem.PartType = bom.partType;
                            materialIssueItem.UOM = bom.uom;
                            // materialIssueItem.RequiredQty = (bom.quantity * shopOrder.TotalSOReleaseQty);
                            materialIssueItem.RequiredQty = (bom.quantity * shopOrder.ShopOrderItems[i].ReleaseQty);
                            materialIssueItem.IssuedQty = 0;
                            materialIssueItem.MaterialIssuedStatus = IssuedStatus.Open;
                            materialIssueItemList.Add(materialIssueItem);
                        }
                    }
                }

                // Group and assign items to the MaterialIssue object
                var groupedMaterialIssueItems = materialIssueItemList
                    .GroupBy(item => item.PartNumber)
                    .Select(group => new MaterialIssueItem
                    {
                        PartNumber = group.Key,
                        Description = group.First().Description,
                        ProjectNumber = group.First().ProjectNumber,
                        PartType = group.First().PartType,
                        UOM = group.First().UOM,
                        RequiredQty = group.Sum(item => item.RequiredQty),
                        IssuedQty = 0,
                        MaterialIssuedStatus = IssuedStatus.Open,
                    })
                    .ToList();
                materialIssue.materialIssueItems = groupedMaterialIssueItems;

                // Create only one MaterialIssue record
                await _materialIssueRepository.CreateMaterialIssue(materialIssue);
                _materialIssueRepository.SaveAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateShopOrder action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong {ex.Message},{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //private async Task<IActionResult> CreateMaterialIssueDetails(ShopOrder shopOrder)
        //{
        //    ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();
        //    try
        //    {
        //        dynamic bomData = null;
        //        for (int i = 0; i < shopOrder.ShopOrderItems.Count(); i++)
        //        {
        //            if (i == 0)
        //            {
        //                var fgNumber = shopOrder.ItemNumber;
        //                decimal bomversion = shopOrder.BomRevisionNo;
        //                var bomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"], "GetProductionBomByItemAndBomVersionNo?", "ItemNumber=", fgNumber, "&bomVersionNo=", bomversion));
        //                var bomDetailsString = await bomDetails.Content.ReadAsStringAsync();
        //                dynamic bomDetailsData = JsonConvert.DeserializeObject(bomDetailsString);
        //                bomData = bomDetailsData.data;
        //            }

        //            if (bomData != null)
        //            {
        //                MaterialIssue materialIssue = new MaterialIssue();
        //                materialIssue.ShopOrderNumber = shopOrder.ShopOrderNumber;
        //                materialIssue.ShopOrderDate = shopOrder.CreatedOn;
        //                materialIssue.ProjectType = ProjectType.RFQ;
        //                materialIssue.ItemType = shopOrder.ItemType;
        //                materialIssue.ShopOrderQty = shopOrder.TotalSOReleaseQty;
        //                materialIssue.ItemNumber = shopOrder.ItemNumber;
        //                materialIssue.MaterialIssuedStatus = IssuedStatus.Open;
        //                List<MaterialIssueItem> materialIssueItemList = new List<MaterialIssueItem>();
        //                foreach (var bom in bomData.enggChildItemDtos)
        //                {
        //                    var projectNo = shopOrder.ShopOrderItems[i].ProjectNumber;
        //                    MaterialIssueItem materialIssueItem = new MaterialIssueItem();
        //                    materialIssueItem.PartNumber = bom.itemNumber;
        //                    materialIssueItem.Description = bom.description;
        //                    materialIssueItem.ProjectNumber = projectNo;
        //                    materialIssueItem.PartType = bom.partType;
        //                    materialIssueItem.UOM = bom.uom;
        //                    materialIssueItem.RequiredQty = (bom.quantity * shopOrder.TotalSOReleaseQty);
        //                    materialIssueItem.IssuedQty = 0;
        //                    materialIssueItem.MaterialIssuedStatus = IssuedStatus.Open; 
        //                    materialIssueItemList.Add(materialIssueItem);


        //                }

        //                var groupedMaterialIssueItems = materialIssueItemList
        //                    .GroupBy(item => item.PartNumber)
        //                    .Select(group => new MaterialIssueItem
        //                    {
        //                        PartNumber = group.Key,
        //                        Description = group.First().Description,
        //                        ProjectNumber = group.First().ProjectNumber,
        //                        PartType = group.First().PartType,
        //                        UOM = group.First().UOM,
        //                        RequiredQty = group.Sum(item => item.RequiredQty),
        //                        IssuedQty = 0, // Assuming IssuedQty remains 0 for grouped items
        //                        MaterialIssuedStatus = IssuedStatus.Open
        //                    })
        //                    .ToList();
        //                materialIssue.materialIssueItems = groupedMaterialIssueItems;
        //                await _materialIssueRepository.CreateMaterialIssue(materialIssue);
        //                _materialIssueRepository.SaveAsync();
        //            }
        //        } 
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside CreateShopOrder action: {ex.Message},{ex.InnerException}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong {ex.Message},{ex.InnerException}";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //  }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShopOrder(int id, [FromBody] ShopOrderUpdateDto ShopOrderDtoUpdate)
        {
            ServiceResponse<ShopOrderUpdateDto> serviceResponse = new ServiceResponse<ShopOrderUpdateDto>();

            try
            {
                if (ShopOrderDtoUpdate is null)
                {
                    _logger.LogError("ShopOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ShopOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShopOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ShopOrder object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var shopOrderDetailById = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrderDetailById is null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id={id} hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var shopOrders = _mapper.Map<ShopOrder>(shopOrderDetailById);
                var shopOrderItemDto = ShopOrderDtoUpdate.ShopOrderItems;
                var shopOrderItemList = new List<ShopOrderItem>();
                if (shopOrderItemDto != null)
                    for (int i = 0; i < shopOrderItemDto.Count; i++)
                    {
                        ShopOrderItem shoporderItemDetail = _mapper.Map<ShopOrderItem>(shopOrderItemDto[i]);
                        shopOrderItemList.Add(shoporderItemDetail);

                    }
                shopOrders.ShopOrderItems = shopOrderItemList;
                var updateShopOrder = _mapper.Map(ShopOrderDtoUpdate, shopOrderDetailById);
                string result = await _shopOrderRepository.UpdateShopOrder(updateShopOrder);
                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrder Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShopOrder(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrderDetailById = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrderDetailById == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shopOrderDetailById.IsDeleted = true;
                string result = await _shopOrderRepository.UpdateShopOrder(shopOrderDetailById);
                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrder Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private string Delete(ShopOrder shopOrders)
        {
            throw new NotImplementedException();
        }

        [HttpGet("salesOrderNo")]
        public async Task<IActionResult> GetShopOrderBySalesOrderNo(string salesOrderNo)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrderBySalesOrderNo = await _shopOrderRepository.GetShopOrderBySalesOrderNo(salesOrderNo);
                if (shopOrderBySalesOrderNo == null)
                {
                    _logger.LogError($"ShopOrder with id: {salesOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with salesOrderNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {salesOrderNo}");
                    var result = _mapper.Map<ShopOrderDto>(shopOrderBySalesOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderBySalesOrderNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        //[HttpGet("salesOrderNo")]
        [HttpGet]
        public async Task<IActionResult> GetShopOrderNoListBySalesOrderNo(string salesOrderNo, string itemNumber)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();

            try
            {
                var shopOrderNoList = await _shopOrderRepository.GetShopOrderNoListBySalesOrderNo(salesOrderNo, itemNumber);
                if (shopOrderNoList == null)
                {
                    _logger.LogError($"ShopOrder with id: {salesOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with salesOrderNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder Number List with id: {salesOrderNo}");
                    //var result = _mapper.Map<ShopOrderDto>(shopOrderBySalesOrderNo);
                    //serviceResponse.Data = shopOrderNoList;
                    //serviceResponse.Message = "ShopOrderBySalesOrderNo Successfully Returned";
                    //serviceResponse.Success = true;
                    //serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(shopOrderNoList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderNoListBySalesOrderNo for SalesOrderNo {salesOrderNo} action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }




        [HttpGet("shopOrderNo")]
        public async Task<IActionResult> GetShopOrderByShopOrderNo(string shopOrderNo)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrderByShopOrderNo = await _shopOrderRepository.GetShopOrderByShopOrderNo(shopOrderNo);
                if (shopOrderByShopOrderNo == null)
                {
                    _logger.LogError($"ShopOrder with id: {shopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with shopOrderNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {shopOrderNo}");
                    var result = _mapper.Map<ShopOrderDto>(shopOrderByShopOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderByShopOrderNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetShopOrderByItemType(string itemType)
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();

            try
            {
                var shopOrderByItemType = await _shopOrderRepository.GetShopOrderByItemType(itemType);
                if (shopOrderByItemType == null)
                {
                    _logger.LogError($"ShopOrder with id: {itemType}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with itemType hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {itemType}");
                    var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(shopOrderByItemType);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderByItemType Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderByItemType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetShopOrderByFGNo(string fGNumber)
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();

            try
            {
                var shopOrderByFGNo = await _shopOrderRepository.GetShopOrderByFGNo(fGNumber);
                if (shopOrderByFGNo == null)
                {
                    _logger.LogError($"ShopOrder with id: {fGNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with fGNumber hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {fGNumber}");
                    var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(shopOrderByFGNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderByFGNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderByFGNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderByFGNoAndSANo(string fGNumber, string sANumber)
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();

            try
            {
                var shopOrderByFGNoAndSANo = await _shopOrderRepository.GetShopOrderByFGNoAndSANo(fGNumber, sANumber);
                if (shopOrderByFGNoAndSANo == null)
                {
                    _logger.LogError($"ShopOrder with id: {fGNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"shopOrderByFGNoAndSANo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {fGNumber}");
                    var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(shopOrderByFGNoAndSANo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderByFGNoAndSANo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderByFGNoAndSANo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOpenShopOrders()
        {
            ServiceResponse<IEnumerable<ShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderDto>>();

            try
            {
                var openShopOrderDetials = await _shopOrderRepository.GetAllOpenShopOrders();
                _logger.LogInfo("Returned all ShopOrders");
                var result = _mapper.Map<IEnumerable<ShopOrderDto>>(openShopOrderDetials);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenShopOrderDetails Successfully ";
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

        [HttpGet]
        public async Task<IActionResult> GetAllActiveShopOrderNoList()
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();
            try
            {
                var listOfActiveShopOrderNo = await _shopOrderRepository.GetAllActiveShopOrderNoList();

                var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(listOfActiveShopOrderNo);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveShopOrderNoList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveShopOrderNoList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShopOrderIdNameList()
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();
            try
            {
                var listOfShopOrderNo = await _shopOrderRepository.GetAllShopOrderIdNameList();

                var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(listOfShopOrderNo);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ShopOrderIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllShopOrderIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ShortCloseShopOrder(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shortCloseShopOrderById = await _shopOrderRepository.GetShopOrderById(id);
                if (shortCloseShopOrderById == null)
                {
                    _logger.LogError($"ShortCloseShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShortCloseShopOrder with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shortCloseShopOrderById.IsShortClosed = true;
                shortCloseShopOrderById.ShortClosedBy = _createdBy;
                shortCloseShopOrderById.ShortClosedOn = DateTime.Now;
                string result = await _shopOrderRepository.UpdateShopOrder(shortCloseShopOrderById);
                //Get Matterial Issue Details
                var materialIssue = await _materialIssueRepository.GetMaterialIssueByShopOrderNo(shortCloseShopOrderById.ShopOrderNumber);
                materialIssue.IsShortClosed = true;
                await _materialIssueRepository.UpdateMaterialIssue(materialIssue);

                _materialIssueRepository.SaveAsync();
                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrder have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ShortCloseShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ShortCloseShopOrderItemSatusByShopOrderItemId(int soItemId)
        {
            ServiceResponse<ShopOrderItemDto> serviceResponse = new ServiceResponse<ShopOrderItemDto>();

            try
            {
                var soItemDetailBySOItemId = await _shopOrderItemRepository.GetShopOrderItemById(soItemId);
                if (soItemDetailBySOItemId == null)
                {
                    _logger.LogError($"ShopOrderItems with soItemId: {soItemId}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderItems with soItemId hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

                soItemDetailBySOItemId.Status = OrderStatus.ShortClose;
                string result = await _shopOrderItemRepository.UpdateShopOrderItem(soItemDetailBySOItemId);
                _shopOrderItemRepository.SaveAsync();

                //Update ShopOrder Table Status
                var shopOrderItemOpenStatuscount = await _shopOrderItemRepository.GetShopOrderItemOpenStatusCount(soItemDetailBySOItemId.ShopOrderId);

                if (shopOrderItemOpenStatuscount == 0)
                {
                    var shopOrderDetails = await _shopOrderRepository.GetShopOrderById(soItemDetailBySOItemId.ShopOrderId);
                    shopOrderDetails.Status = OrderStatus.ShortClose;
                    await _shopOrderRepository.UpdateShopOrder(shopOrderDetails);
                    _shopOrderRepository.SaveAsync();
                }
                else
                {
                    var shopOrderDetails = await _shopOrderRepository.GetShopOrderById(soItemDetailBySOItemId.ShopOrderId);
                    shopOrderDetails.Status = OrderStatus.PartiallyClosed;
                    await _shopOrderRepository.UpdateShopOrder(shopOrderDetails);
                    _shopOrderRepository.SaveAsync();
                }

                //Update PendingShopOrderConfirmationQty in SalesOrder Table

                var ShopOrderDetails = await _shopOrderRepository.GetShopOrderById(soItemDetailBySOItemId.ShopOrderId);
                var pendingSoConfirmationQty = ShopOrderDetails.TotalSOReleaseQty - ShopOrderDetails.WipQty;

                UpdateShopOrderQtyDto updateShopOrderQtyDto = new UpdateShopOrderQtyDto();
                updateShopOrderQtyDto.FGItemNumber = soItemDetailBySOItemId.FGItemNumber;
                updateShopOrderQtyDto.ProjectNumber = soItemDetailBySOItemId.ProjectNumber;
                updateShopOrderQtyDto.SalesOrderNumber = soItemDetailBySOItemId.SalesOrderNumber;
                updateShopOrderQtyDto.PendingSoConfirmationQty = pendingSoConfirmationQty;

                var jsons = JsonConvert.SerializeObject(updateShopOrderQtyDto);
                var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                var responses = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "UpdatePendingShopOrderQty?"), datas);

                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrderItems Status have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ShortCloseShopOrderItemSatusByShopOrderItemId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //SA ShopOrder


        [HttpGet]
        public async Task<IActionResult> GetSAShopOrderBalanceQty(string fgItemNumber, string saItemNumber, string projectNumber, string salesOrderNumber)
        {
            ServiceResponse<decimal?> serviceResponse = new ServiceResponse<decimal?>();
            try
            {
                var notShortCloseQty = await _shopOrderItemRepository.GetNotShortCloseQty(fgItemNumber, saItemNumber, projectNumber, salesOrderNumber);
                if (notShortCloseQty == null)
                {
                    _logger.LogError($"ShopOrder Release Quantity is getting Null Values");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder Release Quantity is getting Null Values";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

                serviceResponse.Data = notShortCloseQty;
                serviceResponse.Message = "Get ShopOrder Qunatity successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ShortCloseShopOrderItemSatusByShopOrderItemId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> PickListReport(string? ShopOrderNumber)
        {
            ServiceResponse<List<PickListGetDTO>?> serviceResponse = new ServiceResponse<List<PickListGetDTO>?>();
            try
            {
                if (ShopOrderNumber == null)
                {
                    _logger.LogError($"ShopOrder is getting Null Values");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder is getting Null Values";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var piclists = await _shopOrderRepository.GetPickListReport(ShopOrderNumber);
                List<PickListGetDTO>? result = new List<PickListGetDTO>();
                foreach (var pic in piclists)
                {
                    PickListGetDTO pickListGetDTO = _mapper.Map<PickListGetDTO>(pic);
                    List<Invdata>? invdatas = JsonConvert.DeserializeObject<List<Invdata>>(pic.InventoryData);
                    pickListGetDTO.InventoryDatas = invdatas;
                    result.Add(pickListGetDTO);
                }
                serviceResponse.Data = result;
                serviceResponse.Message = "Get PickListReport successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside PickListReport action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
