using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Contracts;
using Newtonsoft.Json;
using Tips.Warehouse.Api.Repository;
using System.Security.Claims;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Entities.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OpenDeliveryOrderController : ControllerBase
    {
        private IOpenDeliveryOrderRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IInventoryRepository _inventoryRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IOpenDeliveryOrderHistoryRepository _openDeliveryOrderHistoryRepository;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly IHttpClientFactory _clientFactory;
        public OpenDeliveryOrderController(IConfiguration config, IHttpClientFactory clientFactory, IOpenDeliveryOrderHistoryRepository openDeliveryOrderHistoryRepository, IInventoryTranctionRepository inventoryTranctionRepository, IOpenDeliveryOrderRepository repository, IInventoryRepository inventoryRepository, ILoggerManager logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _openDeliveryOrderHistoryRepository = openDeliveryOrderHistoryRepository;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        // GET: api/<OpenDeliveryOrderController>
        [HttpGet]
        public async Task<IActionResult> GetAllOpenDeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderDto>>();

            try
            {
                var getAllOpenDeliveryOrderDetails = await _repository.GetAllOpenDeliveryOrders(pagingParameter, searchParams);

                var metadata = new
                {
                    getAllOpenDeliveryOrderDetails.TotalCount,
                    getAllOpenDeliveryOrderDetails.PageSize,
                    getAllOpenDeliveryOrderDetails.CurrentPage,
                    getAllOpenDeliveryOrderDetails.HasNext,
                    getAllOpenDeliveryOrderDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all OpenDeliveryOrders");
                var result = _mapper.Map<IEnumerable<OpenDeliveryOrderDto>>(getAllOpenDeliveryOrderDetails);
                foreach (var ODOdetails in result)
                {
                    if(ODOdetails.DOType!= "returnable") ODOdetails.AllowReturnODO= false;
                    else ODOdetails.AllowReturnODO=(ODOdetails.OpenDeliveryOrderParts.Sum(x=>x.DispatchQty)>0) ? true : false;
                }
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenDeliveryOrders Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOpenDeliveryOrders API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOpenDeliveryOrders API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> OpenDeliveryOrderSPReport([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderSPReport>>();
            try
            {
                var products = await _repository.OpenDeliveryOrderSPReport(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all OpenDeliveryOrderSPReport");

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenDeliveryOrderSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in OpenDeliveryOrderSPReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in OpenDeliveryOrderSPReport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpenDeliveryOrderById(int id)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                var getOpenDeliveryOrderById = await _repository.GetOpenDeliveryOrderById(id);

                if (getOpenDeliveryOrderById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrdersDetails with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrdersDetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned OpenDeliveryOrdersDetails with id: {id}");
                    //var result = _mapper.Map<OpenDeliveryOrderDto>(getOpenDeliveryOrderById);

                    OpenDeliveryOrderDto OpenDeliveryOrderDto = _mapper.Map<OpenDeliveryOrderDto>(getOpenDeliveryOrderById);

                    List<OpenDeliveryOrderPartsDto> OpenDeliveryOrderItemsDtoList = new List<OpenDeliveryOrderPartsDto>();

                    if (getOpenDeliveryOrderById.OpenDeliveryOrderParts != null)
                    {

                        foreach (var openDeliveryOrderitemDetails in getOpenDeliveryOrderById.OpenDeliveryOrderParts)
                        {
                            OpenDeliveryOrderPartsDto openDeliveryOrderItemsDtos = _mapper.Map<OpenDeliveryOrderPartsDto>(openDeliveryOrderitemDetails);
                            openDeliveryOrderItemsDtos.QtyDistribution = _mapper.Map<List<OpenDeliveryOrderPartsQtyDistributionDto>>(openDeliveryOrderitemDetails.QtyDistribution);
                            OpenDeliveryOrderItemsDtoList.Add(openDeliveryOrderItemsDtos);
                        }
                    }
                    OpenDeliveryOrderDto.OpenDeliveryOrderParts = OpenDeliveryOrderItemsDtoList;

                    serviceResponse.Data = OpenDeliveryOrderDto;
                    serviceResponse.Message = $"Returned OpenDeliveryOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOpenDeliveryOrderById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOpenDeliveryOrderById API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("itemNumber")]
        public async Task<IActionResult> GetStockDetailsByItemNo(string itemNumber)
        {
            ServiceResponse<ODODetailsDto> serviceResponse = new ServiceResponse<ODODetailsDto>();
            try
            {
                var oDODetailsById = await _repository.GetODODetailsByItemNo(itemNumber);

                if (oDODetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrder with itemNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrder with id: {itemNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    ODODetailsDto OdoDetailsDto = new ODODetailsDto();
                    OdoDetailsDto.ItemNumber = oDODetailsById.ItemNumber;
                    OdoDetailsDto.ItemType = oDODetailsById.ItemType;
                    OdoDetailsDto.UOM = oDODetailsById.UOM;
                    OdoDetailsDto.StockAvailable = oDODetailsById.StockAvailable;

                    var warehouseODODetails = await _repository.GetWarehouseODOByItemNo(itemNumber);

                    foreach (var warehouse in warehouseODODetails)
                    {
                        warehouse.LocationDetails = await _repository.GetLocationODOByItemNo(itemNumber, warehouse.WarehouseName);
                    }
                    OdoDetailsDto.WarehouseDetails = warehouseODODetails;

                    _logger.LogInfo($"Returned OpenDeliveryOrder with id: {itemNumber}");
                    var result = _mapper.Map<ODODetailsDto>(OdoDetailsDto);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Stock Details Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetStockDetailsByItemNo API for the following itemNumber:{itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetStockDetailsByItemNo API for the following itemNumber:{itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        //date search opendelivery order api

        [HttpGet]
        public async Task<IActionResult> SearchOpenDeliveryOrderDate([FromQuery] SearchsDateParms searchDateParam)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderDto>>();
            try
            {
                var openDeliveryOrders = await _repository.SearchOpenDeliveryOrderDate(searchDateParam);
                var result = _mapper.Map<IEnumerable<OpenDeliveryOrderDto>>(openDeliveryOrders);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenDeliveryOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchOpenDeliveryOrderDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchOpenDeliveryOrderDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchOpenDeliveryOrder([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderDto>>();
            try
            {
                var openDeliveyOrderList = await _repository.SearchOpenDeliveryOrder(searchParams);

                _logger.LogInfo("Returned all OpenDeliveryOrder");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<OpenDeliveryOrder, OpenDeliveryOrderDto>().ReverseMap()
                    .ForMember(dest => dest.OpenDeliveryOrderParts, opt => opt.MapFrom(src => src.OpenDeliveryOrderParts));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<OpenDeliveryOrderDto>>(openDeliveyOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenDeliveryOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchOpenDeliveryOrder API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchOpenDeliveryOrder API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllOpenDeliveryOrderWithItems([FromBody] OpenDeliveryOrderSearchDto openDeliveryOrderSearch)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderDto>>();
            try
            {
                var openDeliveryOrders = await _repository.GetAllOpenDeliveryOrderWithItems(openDeliveryOrderSearch);
                _logger.LogInfo("Returned all openDeliveryOrders");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<OpenDeliveryOrderDto, OpenDeliveryOrder>().ReverseMap()
                    .ForMember(dest => dest.OpenDeliveryOrderParts, opt => opt.MapFrom(src => src.OpenDeliveryOrderParts));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<OpenDeliveryOrderDto>>(openDeliveryOrders);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenDeliveryOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOpenDeliveryOrderWithItems API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOpenDeliveryOrderWithItems API : \n {ex.Message}";
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
        public async Task<IActionResult> CreateOpenDeliveryOrder([FromBody] OpenDeliveryOrderDtoPost openDeliveryOrderDtoPost)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                string serverKey = GetServerKey();
                if (openDeliveryOrderDtoPost is null)
                {
                    _logger.LogError("OpenDeliveryOrderDetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenDeliveryOrderDetails object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid OpenDeliveryOrderDetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OpenDeliveryOrderDetails object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var openDeliveryOrderparts = _mapper.Map<List<OpenDeliveryOrderParts>>(openDeliveryOrderDtoPost.OpenDeliveryOrderParts);
                var openDeliveryOrderitemsList = openDeliveryOrderDtoPost.OpenDeliveryOrderParts;
                var openDeliveryorder = _mapper.Map<OpenDeliveryOrder>(openDeliveryOrderDtoPost);
                var openDeliveryOrderItemsDtoList = new List<OpenDeliveryOrderParts>();
                openDeliveryorder.OpenDeliveryOrderParts = openDeliveryOrderparts;
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));
                string? odoNumber = null;
                if (serverKey == "trasccon")
                {
                    var dateFormat = days + months + years;
                    odoNumber = await _repository.GenerateODONumber();
                    openDeliveryorder.OpenDONumber = dateFormat + odoNumber;
                }
                else if (serverKey == "keus")
                {
                    var dateFormat = days + months + years;
                    odoNumber = await _repository.GenerateODONumber();
                    openDeliveryorder.OpenDONumber = dateFormat + odoNumber;
                }
                else
                {
                    odoNumber = await _repository.GenerateODONumberAvision();
                    openDeliveryorder.OpenDONumber = odoNumber;
                }

                if (openDeliveryOrderitemsList != null)
                {
                    for (int i = 0; i < openDeliveryOrderitemsList.Count; i++)
                    {
                        OpenDeliveryOrderParts OpenDeliveryOrderItemsDetails = _mapper.Map<OpenDeliveryOrderParts>(openDeliveryOrderitemsList[i]);
                        OpenDeliveryOrderItemsDetails.QtyDistribution = _mapper.Map<List<OpenDeliveryOrderPartsQtyDistribution>>(openDeliveryOrderitemsList[i].QtyDistribution);
                        OpenDeliveryOrderItemsDetails.InitialDispatchQty = OpenDeliveryOrderItemsDetails.DispatchQty;
                        OpenDeliveryOrderItemsDetails.ODONumber = openDeliveryorder.OpenDONumber;
                        openDeliveryOrderItemsDtoList.Add(OpenDeliveryOrderItemsDetails);

                        var distriution = _mapper.Map<List<OpenDeliveryOrderPartsQtyDistribution>>(openDeliveryOrderitemsList[i].QtyDistribution);
                        //Update Inventory balanced Quantity 
                        await _inventoryRepository.UpdateInventoryforODO(distriution);

                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request1.Headers.Add("Authorization", token1);

                        var itemMasterObjectResult = await client1.SendAsync(request1);

                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        var itemMasterObjectData = JsonConvert.DeserializeObject<ReturnBTONumberInvDetails>(itemMasterObjectString);
                        var itemMasterObject = itemMasterObjectData.data;

                        ////Add BTO Detail Into Inventory transaction Table
                        foreach (var eachbin in OpenDeliveryOrderItemsDetails.QtyDistribution)
                        {
                            //InventoryTranction inventoryTranction = new InventoryTranction();
                            //inventoryTranction.PartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                            //inventoryTranction.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                            //inventoryTranction.LotNumber = eachbin.LotNumber;
                            //inventoryTranction.ProjectNumber = eachbin.ProjectNumber;
                            //inventoryTranction.PartType = openDeliveryOrderItemsDtoList[i].ItemType;
                            //inventoryTranction.Description = openDeliveryOrderItemsDtoList[i].ItemDescription;
                            //inventoryTranction.Issued_Quantity = eachbin.DistributingQty;
                            //inventoryTranction.IsStockAvailable = true;
                            //inventoryTranction.UOM = openDeliveryOrderItemsDtoList[i].UOM;
                            //inventoryTranction.Issued_DateTime = DateTime.Now;
                            //inventoryTranction.Issued_By = _createdBy;
                            //inventoryTranction.ReferenceID = openDeliveryOrderItemsDtoList[i].ODONumber;
                            //inventoryTranction.ReferenceIDFrom = "Open Delivery Order";
                            //inventoryTranction.From_Location = eachbin.Location;
                            //inventoryTranction.TO_Location = "ODO";
                            //inventoryTranction.Warehouse = eachbin.Warehouse;
                            //inventoryTranction.Remarks = "Create ODO";
                            //var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);

                            InventoryTranction inventoryTranction = new InventoryTranction();
                            inventoryTranction.PartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                            inventoryTranction.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                            inventoryTranction.ProjectNumber = eachbin.ProjectNumber;
                            inventoryTranction.Description = openDeliveryOrderItemsDtoList[i].ItemDescription;
                            inventoryTranction.Issued_Quantity = eachbin.DistributingQty;
                            inventoryTranction.LotNumber = eachbin.LotNumber;
                            inventoryTranction.UOM = openDeliveryOrderItemsDtoList[i].UOM;
                            inventoryTranction.Unit = _unitname;
                            inventoryTranction.IsStockAvailable = true;
                            inventoryTranction.Warehouse = eachbin.Warehouse;
                            inventoryTranction.From_Location = eachbin.Location;
                            inventoryTranction.TO_Location = "ODO";
                            inventoryTranction.PartType = openDeliveryOrderItemsDtoList[i].ItemType;
                            inventoryTranction.ReferenceID = openDeliveryOrderItemsDtoList[i].ODONumber;
                            inventoryTranction.ReferenceIDFrom = "LocationTransferPartNo";
                            inventoryTranction.Remarks = "LocationTransferPartNo Done";
                            inventoryTranction.TransactionType = InventoryType.Outward;
                            await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranction);

                           // await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                            //_inventoryTranctionRepository.SaveAsync();

                            //// Add Bto detail in to opendeliveryorderhistory table

                            OpenDeliveryOrderHistory openDeliveryOrderHistory = new OpenDeliveryOrderHistory();
                            openDeliveryOrderHistory.ODONumber = openDeliveryorder.OpenDONumber;
                            openDeliveryOrderHistory.LotNumber = eachbin.LotNumber;
                            openDeliveryOrderHistory.CustomerName = openDeliveryorder.CustomerName;
                            openDeliveryOrderHistory.CustomerAliasName = openDeliveryorder.CustomerAliasName;
                            openDeliveryOrderHistory.CustomerId = openDeliveryorder.CustomerId;
                            openDeliveryOrderHistory.Description = openDeliveryorder.Description;
                            openDeliveryOrderHistory.ResponsiblePerson = openDeliveryorder.ResponsiblePerson;
                            openDeliveryOrderHistory.ReasonForIssuingStock = openDeliveryorder.ReasonforIssuingStock;
                            openDeliveryOrderHistory.IssuedTo = openDeliveryorder.IssuedTo;
                            openDeliveryOrderHistory.ODOType = openDeliveryorder.IssuedTo;
                            openDeliveryOrderHistory.ODODate = openDeliveryorder.OpenDODate;
                            openDeliveryOrderHistory.ItemNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                            openDeliveryOrderHistory.ItemDescription = openDeliveryOrderItemsDtoList[i].ItemDescription;
                            openDeliveryOrderHistory.ItemType = openDeliveryOrderItemsDtoList[i].ItemType;
                            openDeliveryOrderHistory.UnitPrice = openDeliveryOrderItemsDtoList[i].UnitPrice;
                            openDeliveryOrderHistory.UOC = openDeliveryOrderItemsDtoList[i].UOC;
                            openDeliveryOrderHistory.UOM = openDeliveryOrderItemsDtoList[i].UOM;
                            openDeliveryOrderHistory.Warehouse = eachbin.Warehouse;
                            openDeliveryOrderHistory.StockAvailable = openDeliveryOrderItemsDtoList[i].StockAvailable;
                            openDeliveryOrderHistory.Location = eachbin.Location;
                            openDeliveryOrderHistory.LocationStock = openDeliveryOrderItemsDtoList[i].LocationStock;
                            openDeliveryOrderHistory.DispatchQty = eachbin.DistributingQty;
                            openDeliveryOrderHistory.SerialNo = openDeliveryOrderItemsDtoList[i].SerialNo;
                            openDeliveryOrderHistory.Unit = openDeliveryOrderItemsDtoList[i].SerialNo;
                            openDeliveryOrderHistory.UniqeId = openDeliveryOrderItemsDtoList[i].SerialNo;
                            //openDeliveryOrderHistory.CreatedBy = openDeliveryOrderItemsDtoList[i].CreatedBy;
                            //openDeliveryOrderHistory.LastModifiedOn = openDeliveryOrderItemsDtoList[i].LastModifiedOn;
                            openDeliveryOrderHistory.Remark = "From Create ODO";

                            var openDeliveryOrderHistoryDetails = _mapper.Map<OpenDeliveryOrderHistory>(openDeliveryOrderHistory);


                            await _openDeliveryOrderHistoryRepository.CreateOpenDeliveryOrderHistory(openDeliveryOrderHistoryDetails);
                            // _openDeliveryOrderHistoryRepository.SaveAsync();
                        }
                    }
                }
                openDeliveryorder.OpenDeliveryOrderParts = openDeliveryOrderItemsDtoList;

                await _repository.CreateOpenDeliveryOrder(openDeliveryorder);

                _repository.SaveAsync();
                _openDeliveryOrderHistoryRepository.SaveAsync();
                _inventoryTranctionRepository.SaveAsync();
                _inventoryRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OpenDeliveryorder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetOpenDeliveryOrderById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateOpenDeliveryOrder API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateOpenDeliveryOrder API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateOpenDeliveryOrder_AV([FromBody] OpenDeliveryOrderDtoPost openDeliveryOrderDtoPost)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                string serverKey = GetServerKey();
                if (openDeliveryOrderDtoPost is null)
                {
                    _logger.LogError("OpenDeliveryOrderDetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenDeliveryOrderDetails object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid OpenDeliveryOrderDetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OpenDeliveryOrderDetails object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var openDeliveryOrderparts = _mapper.Map<IEnumerable<OpenDeliveryOrderParts>>(openDeliveryOrderDtoPost.OpenDeliveryOrderParts);

                var openDeliveryOrderitemsList = openDeliveryOrderDtoPost.OpenDeliveryOrderParts;

                var openDeliveryorder = _mapper.Map<OpenDeliveryOrder>(openDeliveryOrderDtoPost);
                var openDeliveryOrderItemsDtoList = new List<OpenDeliveryOrderParts>();

                openDeliveryorder.OpenDeliveryOrderParts = openDeliveryOrderparts.ToList();
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                if (serverKey == "trasccon")
                {
                    var dateFormat = days + months + years;
                    var odoNumber = await _repository.GenerateODONumber();
                    openDeliveryorder.OpenDONumber = dateFormat + odoNumber;
                }
                else if (serverKey == "keus")
                {
                    var dateFormat = days + months + years;
                    var odoNumber = await _repository.GenerateODONumber();
                    openDeliveryorder.OpenDONumber = dateFormat + odoNumber;
                }
                else
                {
                    var odoNumber = await _repository.GenerateODONumberAvision();
                    openDeliveryorder.OpenDONumber = odoNumber;
                }

                if (openDeliveryOrderitemsList != null)
                {

                    for (int i = 0; i < openDeliveryOrderitemsList.Count; i++)
                    {
                        OpenDeliveryOrderParts OpenDeliveryOrderItemsDetails = _mapper.Map<OpenDeliveryOrderParts>(openDeliveryOrderitemsList[i]);
                        //OpenDeliveryOrderItemsDetails.QtyDistribution = _mapper.Map<List<OpenDeliveryOrderPartsQtyDistribution>>(openDeliveryOrderitemsList[i].QtyDistribution);
                        OpenDeliveryOrderItemsDetails.ODONumber = openDeliveryorder.OpenDONumber;
                        openDeliveryOrderItemsDtoList.Add(OpenDeliveryOrderItemsDetails);

                        //Update Inventory balanced Quantity 
                        //await _inventoryRepository.UpdateInventoryforODO(OpenDeliveryOrderItemsDetails.QtyDistribution);


                        //Old Code [
                        var PartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                        var getInventoryFGDetailsByItemnumber = await _inventoryRepository.GetInventoryDetailsByItemNoandPartTypes(PartNumber);
                        decimal Quantity = Convert.ToDecimal(openDeliveryOrderitemsList[i].DispatchQty);

                        if (getInventoryFGDetailsByItemnumber != null)
                        {
                            foreach (var item in getInventoryFGDetailsByItemnumber)
                            {
                                if (Quantity != 0 && item.Balance_Quantity >= Quantity)
                                {
                                    item.Balance_Quantity = item.Balance_Quantity - Quantity;
                                    Quantity = 0;
                                    if (item.Balance_Quantity == 0)
                                    {
                                        item.IsStockAvailable = false;
                                    }

                                }
                                if (Quantity != 0 && item.Balance_Quantity < Quantity)
                                {
                                    Quantity = Quantity - item.Balance_Quantity;
                                    item.Balance_Quantity = 0;
                                    item.IsStockAvailable = false;
                                }

                                _inventoryRepository.UpdateInventory(item);
                                _inventoryRepository.SaveAsync();

                                if (Quantity <= 0)
                                {
                                    break;
                                }
                            }
                        }
                        //else
                        //{
                        //    Inventory inventory = new Inventory();
                        //    inventory.PartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                        //    inventory.MftrPartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                        //    inventory.Description = openDeliveryOrderItemsDtoList[i].ItemDescription;
                        //    inventory.ProjectNumber = "";
                        //    inventory.Balance_Quantity = openDeliveryOrderItemsDtoList[i].DispatchQty;
                        //    inventory.UOM = openDeliveryOrderItemsDtoList[i].UOM;
                        //    inventory.IsStockAvailable = true;
                        //    inventory.Warehouse = openDeliveryOrderItemsDtoList[i].Warehouse;
                        //    inventory.Location = openDeliveryOrderItemsDtoList[i].Location;
                        //    inventory.GrinNo = openDeliveryorder.OpenDONumber;
                        //    inventory.GrinPartId = 0;
                        //    inventory.PartType = openDeliveryOrderItemsDtoList[i].ItemType;
                        //    inventory.GrinMaterialType = "";
                        //    inventory.ReferenceID = openDeliveryorder.OpenDONumber;
                        //    inventory.ReferenceIDFrom = "Create Open Delivery Order";
                        //    inventory.shopOrderNo = "";

                        //    await _inventoryRepository.CreateInventory(inventory);
                        //    _inventoryRepository.SaveAsync();
                        //}
                        //]

                        ////Add BTO Detail Into Inventory transaction Table

                        //InventoryTranction inventoryTranction = new InventoryTranction();
                        //inventoryTranction.PartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                        //inventoryTranction.MftrPartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                        //inventoryTranction.Description = openDeliveryOrderItemsDtoList[i].ItemDescription;
                        //inventoryTranction.Issued_Quantity = Convert.ToDecimal(openDeliveryOrderItemsDtoList[i].DispatchQty);
                        //inventoryTranction.UOM = openDeliveryOrderItemsDtoList[i].UOM;
                        //inventoryTranction.Issued_DateTime = DateTime.Now;
                        //inventoryTranction.ReferenceID = openDeliveryorder.OpenDONumber;
                        //inventoryTranction.ReferenceIDFrom = "Open Delivery Order";
                        //inventoryTranction.Issued_By = "Admin";
                        //inventoryTranction.CreatedOn = DateTime.Now;
                        //inventoryTranction.Unit = "Bangalore";
                        //inventoryTranction.CreatedBy = "Admin";
                        //inventoryTranction.LastModifiedBy = "Admin";
                        //inventoryTranction.PartType = openDeliveryOrderItemsDtoList[i].ItemType;
                        //inventoryTranction.LastModifiedOn = DateTime.Now;
                        //inventoryTranction.ModifiedStatus = false;
                        //inventoryTranction.From_Location = openDeliveryOrderItemsDtoList[i].Location;
                        //inventoryTranction.TO_Location = "ODO";
                        //inventoryTranction.Remarks = "Create ODO";

                        //var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                        //await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                        //_inventoryTranctionRepository.SaveAsync();


                        //// Add Bto detail in to opendeliveryorderhistory table

                        OpenDeliveryOrderHistory openDeliveryOrderHistory = new OpenDeliveryOrderHistory();
                        openDeliveryOrderHistory.ODONumber = openDeliveryorder.OpenDONumber;
                        openDeliveryOrderHistory.CustomerName = openDeliveryorder.CustomerName;
                        openDeliveryOrderHistory.CustomerAliasName = openDeliveryorder.CustomerAliasName;
                        openDeliveryOrderHistory.CustomerId = openDeliveryorder.CustomerId;
                        openDeliveryOrderHistory.Description = openDeliveryorder.Description;
                        openDeliveryOrderHistory.ResponsiblePerson = openDeliveryorder.ResponsiblePerson;
                        openDeliveryOrderHistory.ReasonForIssuingStock = openDeliveryorder.ReasonforIssuingStock;
                        openDeliveryOrderHistory.IssuedTo = openDeliveryorder.IssuedTo;
                        openDeliveryOrderHistory.ODOType = openDeliveryorder.IssuedTo;
                        openDeliveryOrderHistory.ODODate = openDeliveryorder.OpenDODate;
                        openDeliveryOrderHistory.ItemNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                        openDeliveryOrderHistory.ItemDescription = openDeliveryOrderItemsDtoList[i].ItemDescription;
                        openDeliveryOrderHistory.ItemType = openDeliveryOrderItemsDtoList[i].ItemType;
                        openDeliveryOrderHistory.UnitPrice = openDeliveryOrderItemsDtoList[i].UnitPrice;
                        openDeliveryOrderHistory.UOC = openDeliveryOrderItemsDtoList[i].UOC;
                        openDeliveryOrderHistory.UOM = openDeliveryOrderItemsDtoList[i].UOM;
                        openDeliveryOrderHistory.Warehouse = openDeliveryOrderItemsDtoList[i].Warehouse;
                        openDeliveryOrderHistory.StockAvailable = openDeliveryOrderItemsDtoList[i].StockAvailable;
                        openDeliveryOrderHistory.Location = openDeliveryOrderItemsDtoList[i].Location;
                        openDeliveryOrderHistory.LocationStock = openDeliveryOrderItemsDtoList[i].LocationStock;
                        openDeliveryOrderHistory.DispatchQty = openDeliveryOrderItemsDtoList[i].DispatchQty;
                        openDeliveryOrderHistory.SerialNo = openDeliveryOrderItemsDtoList[i].SerialNo;
                        openDeliveryOrderHistory.Unit = openDeliveryOrderItemsDtoList[i].SerialNo;
                        openDeliveryOrderHistory.UniqeId = openDeliveryOrderItemsDtoList[i].SerialNo;
                        //openDeliveryOrderHistory.CreatedBy = openDeliveryOrderItemsDtoList[i].CreatedBy;
                        //openDeliveryOrderHistory.LastModifiedOn = openDeliveryOrderItemsDtoList[i].LastModifiedOn;
                        openDeliveryOrderHistory.Remark = "From Create ODO";

                        var openDeliveryOrderHistoryDetails = _mapper.Map<OpenDeliveryOrderHistory>(openDeliveryOrderHistory);


                        await _openDeliveryOrderHistoryRepository.CreateOpenDeliveryOrderHistory(openDeliveryOrderHistoryDetails);
                        _openDeliveryOrderHistoryRepository.SaveAsync();

                    }
                }
                openDeliveryorder.OpenDeliveryOrderParts = openDeliveryOrderItemsDtoList;

                await _repository.CreateOpenDeliveryOrder(openDeliveryorder);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OpenDeliveryorder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetOpenDeliveryOrderById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateOpenDeliveryOrder_AV API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateOpenDeliveryOrder_AV API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> OpenDeliveryOrderSPReportDates([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderSPReport>>();
            try
            {
                var products = await _repository.OpenDeliveryOrderSPReportDates(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrderSPReportDates hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrderSPReportDates hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenDeliveryOrderSPReportDates Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in OpenDeliveryOrderSPReportDates API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in OpenDeliveryOrderSPReportDates API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> OpenDeliveryOrderSPReportDateForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.OpenDeliveryOrderSPReportDateForTrans(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrderSPReportDates hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrderSPReportDates hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenDeliveryOrderSPReportDates Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in OpenDeliveryOrderSPReportDateForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in OpenDeliveryOrderSPReportDateForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> OpenDeliveryOrderSPReportDateForAvi([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderSPReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderSPReportForAvi>>();
            try
            {
                var products = await _repository.OpenDeliveryOrderSPReportDateForAvi(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrderSPReportDates hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrderSPReportDates hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenDeliveryOrderSPReportDates Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in OpenDeliveryOrderSPReportDateForAvi API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in OpenDeliveryOrderSPReportDateForAvi API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpenDeliveryOrder(int id, [FromBody] OpenDeliveryOrderDtoUpdate openDeliveryOrderDtoUpdate)
        {
            ServiceResponse<OpenDeliveryOrderDtoUpdate> serviceResponse = new ServiceResponse<OpenDeliveryOrderDtoUpdate>();

            try
            {
                if (openDeliveryOrderDtoUpdate is null)
                {
                    _logger.LogError("Update OpenDeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update OpenDeliveryOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update OpenDeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update OpenDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getOpenDeliveryOrderbyId = await _repository.GetOpenDeliveryOrderById(id);
                if (getOpenDeliveryOrderbyId is null)
                {
                    _logger.LogError($"Update OpenDeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update OpenDeliveryOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var openDeliveryOrder = _mapper.Map<OpenDeliveryOrder>(getOpenDeliveryOrderbyId);

                var getOldODODeliveryOrderItemsDetails = openDeliveryOrder.OpenDeliveryOrderParts;

                var openDeliveryOrderitemsDto = openDeliveryOrderDtoUpdate.OpenDeliveryOrderParts;
                var openDeliveryOrderitemsList = new List<OpenDeliveryOrderParts>();
                if (openDeliveryOrderitemsDto != null)
                {
                    for (int j = 0; j < getOldODODeliveryOrderItemsDetails.Count; j++)
                    {
                        for (int i = 0; i < openDeliveryOrderitemsDto.Count; i++)
                        {
                            OpenDeliveryOrderParts openDeliveryOrderItems = _mapper.Map<OpenDeliveryOrderParts>(openDeliveryOrderitemsDto[i]);
                            openDeliveryOrderitemsList.Add(openDeliveryOrderItems);
                        }
                    }
                }

                openDeliveryOrder.OpenDeliveryOrderParts = openDeliveryOrderitemsList;
                var updateODODeliveryOrder = _mapper.Map(openDeliveryOrderDtoUpdate, openDeliveryOrder);

                string result = await _repository.UpdateOpenDeliveryOrder(updateODODeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

                //var openDelivaryOrderparts = _mapper.Map<IEnumerable<OpenDeliveryOrderParts>>(openDeliveryOrderDtoUpdate.OpenDeliveryOrderParts);


                //var updateOpenDelivaryOrder = _mapper.Map(openDeliveryOrderDtoUpdate, getOpenDeliveryOrderbyId);


                //updateOpenDelivaryOrder.OpenDeliveryOrderParts = openDelivaryOrderparts.ToList();

                //string result = await _repository.UpdateOpenDeliveryOrder(updateOpenDelivaryOrder);
                //_logger.LogInfo(result);
                //_repository.SaveAsync();
                //serviceResponse.Data = null;
                //serviceResponse.Message = "OpenDelivaryOrder Updated Successfully";
                //serviceResponse.Success = true;
                //serviceResponse.StatusCode = HttpStatusCode.OK;
                //return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateOpenDeliveryOrder API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateOpenDeliveryOrder API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpenDeliveryOrder(int id)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                var deleteOpenDeliveryOrderbyId = await _repository.GetOpenDeliveryOrderById(id);
                if (deleteOpenDeliveryOrderbyId == null)
                {
                    _logger.LogError($"Delete OpenDeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete OpenDeliveryOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteOpenDeliveryOrder(deleteOpenDeliveryOrderbyId);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OpenDeliveryOrder Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteOpenDeliveryOrder API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteOpenDeliveryOrder API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenDeliveryOrderNoList()
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderNoList>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderNoList>>();
            try
            {
                var listOfAllOpenDeliveryOrderNo = await _repository.GetAllOpenDeliveryOrderNoList();
                var result = _mapper.Map<IEnumerable<OpenDeliveryOrderNoList>>(listOfAllOpenDeliveryOrderNo);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All OpenDeliveryOrderNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOpenDeliveryOrderNoList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOpenDeliveryOrderNoList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetODOLotNumberListByODONoAndItemNo(string odoNumber, string itemNumber)
        {
            ServiceResponse<IEnumerable<odoLotNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<odoLotNumberListDto>>();
            try
            {
                var doLotNumberList = await _repository.GetODOLotNumberListByODONoAndItemNo(odoNumber, itemNumber);
                var result = _mapper.Map<IEnumerable<odoLotNumberListDto>>(doLotNumberList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ODOLotNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetODOLotNumberListByODONoAndItemNo API for the following odoNumber:{odoNumber} and itemNumber : {itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetODOLotNumberListByODONoAndItemNo API for the following odoNumber:{odoNumber} and itemNumber : {itemNumber} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> OpenDeliveryOrderSPReportWithParam([FromBody] OpenDeliveryOrderSPReportWithParamDto openDeliveryOrderSPReport)

        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderSPReport>>();
            try
            {
                var products = await _repository.OpenDeliveryOrderSPReportWithParam(openDeliveryOrderSPReport.OpenDoNumber, openDeliveryOrderSPReport.CustomerName, 
                                                                                         openDeliveryOrderSPReport.CustomerAliasName, openDeliveryOrderSPReport.LeadId, 
                                                                                openDeliveryOrderSPReport.IssuedTo, openDeliveryOrderSPReport.KPN, openDeliveryOrderSPReport.MPN,  
                                                                                            openDeliveryOrderSPReport.ODOtype);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrderSPReportDto hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrderSPReportDto hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    //var result = _mapper.Map<IEnumerable<OpenDeliveryOrderSPReport>>(products);

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenDeliveryOrderSPReportDto Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in OpenDeliveryOrderSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in OpenDeliveryOrderSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> OpenDeliveryOrderSPReportWithParamForTrans([FromBody] OpenDeliveryOrderSPReportWithParamForTransDto openDeliveryOrderSPReport)

        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.OpenDeliveryOrderSPReportWithParamForTrans(openDeliveryOrderSPReport.OpenDoNumber, openDeliveryOrderSPReport.CustomerName,openDeliveryOrderSPReport.IssuedTo, 
                                                                                            openDeliveryOrderSPReport.ItemNumber, openDeliveryOrderSPReport.MPN,
                                                                                            openDeliveryOrderSPReport.Warehouse, openDeliveryOrderSPReport.Location,
                                                                                            openDeliveryOrderSPReport.ODOtype, openDeliveryOrderSPReport.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrderSPReportDto hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrderSPReportDto hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    //var result = _mapper.Map<IEnumerable<OpenDeliveryOrderSPReport>>(products);

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenDeliveryOrderSPReportDto Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in OpenDeliveryOrderSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in OpenDeliveryOrderSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> OpenDeliveryOrderSPReportWithParamForAvi([FromBody] OpenDeliveryOrderSPReportWithParamForAviDto openDeliveryOrderSPReport)

        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderSPReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderSPReportForAvi>>();
            try
            {
                var products = await _repository.OpenDeliveryOrderSPReportWithParamForAvi(openDeliveryOrderSPReport.OpenDoNumber, openDeliveryOrderSPReport.VendorName,
                                                                                            openDeliveryOrderSPReport.ItemNumber, 
                                                                                            openDeliveryOrderSPReport.ODOtype);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrderSPReportDto hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrderSPReportDto hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    //var result = _mapper.Map<IEnumerable<OpenDeliveryOrderSPReport>>(products);

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenDeliveryOrderSPReportDto Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in OpenDeliveryOrderSPReportWithParamForAvi API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in OpenDeliveryOrderSPReportWithParamForAvi API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetODOMonthlyConsumptionSPReportWithParam([FromBody] ODOMonthlyConsumptionDto odoMonthlyConsumptionDto)

        {
            ServiceResponse<IEnumerable<ODOMonthlyConsumptionSPReport>> serviceResponse = new ServiceResponse<IEnumerable<ODOMonthlyConsumptionSPReport>>();
            try
            {
                var products = await _repository.GetODOMonthlyConsumptionSPReportWithParam(odoMonthlyConsumptionDto.CustomerId);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ODOMonthlyConsumptionSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ODOMonthlyConsumptionSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ODOMonthlyConsumptionSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetODOMonthlyConsumptionSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetODOMonthlyConsumptionSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetODOMonthlyConsumptionSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<ODOMonthlyConsumptionSPReport>> serviceResponse = new ServiceResponse<IEnumerable<ODOMonthlyConsumptionSPReport>>();
            try
            {
                var products = await _repository.GetODOMonthlyConsumptionSPReportWithDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ODOMonthlyConsumptionSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ODOMonthlyConsumptionSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ODOMonthlyConsumptionSPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetODOMonthlyConsumptionSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetODOMonthlyConsumptionSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetListOfSAODOQtyByItemNo( string saItemNumber, string projectNumber)
        {
            ServiceResponse<ODOQuantityDto> serviceResponse = new ServiceResponse<ODOQuantityDto>();
            try
            {
                var odoItemNoList = await _repository.GetListOfSAODOQtyByItemNo(saItemNumber, projectNumber);


                serviceResponse.Data = odoItemNoList;
                serviceResponse.Message = "Returned  ODOQty  By ItemNo List";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetListOfSAODOQtyByItemNo API for the following saItemNumber:{saItemNumber} and projectNumber : {projectNumber}\n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetListOfSAODOQtyByItemNo API for the following saItemNumber:{saItemNumber} and projectNumber : {projectNumber}\n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListOfODOQtyByItemNo(List<string> itemNumberList ,string projectNumber)
        {
            ServiceResponse<IEnumerable<ODOQuantityDto>> serviceResponse = new ServiceResponse<IEnumerable<ODOQuantityDto>>();
            try
            {
                var odoItemNoList = await _repository.GetListOfODOQtyByItemNo(itemNumberList, projectNumber);

                serviceResponse.Data = odoItemNoList;
                serviceResponse.Message = "Returned  ODOQty  By ItemNo List";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetListOfODOQtyByItemNo API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetListOfODOQtyByItemNo API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
