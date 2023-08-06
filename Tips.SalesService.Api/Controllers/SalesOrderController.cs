using System.Collections;
using System.Net;
using System.Net.Http;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Repository;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        private ISalesOrderRepository _repository;
        private ISalesOrderItemsRepository _salesOrderItemsRepository;
        private ISalesAdditionalChargesRepository _salesAdditionalChargesRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private ISalesOrderHistoryRepository _salesOrderHistory;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public SalesOrderController(IConfiguration config, HttpClient httpClient, ISalesAdditionalChargesRepository salesAdditionalChargesRepository,
            ISalesOrderRepository repository, ISalesOrderHistoryRepository salesOrderHistoryRepository,
            ISalesOrderItemsRepository salesOrderItemsRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _salesOrderItemsRepository = salesOrderItemsRepository;
            _salesOrderHistory = salesOrderHistoryRepository;
            _salesAdditionalChargesRepository = salesAdditionalChargesRepository;
            _httpClient = httpClient;
            _config = config;
        }

        // GET: api/<SalesOrderController>
        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrder([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<SalesOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderDto>>();
            try
            {
                var getAllSalesOrder = await _repository.GetAllSalesOrder(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllSalesOrder.TotalCount,
                    getAllSalesOrder.PageSize,
                    getAllSalesOrder.CurrentPage,
                    getAllSalesOrder.HasNext,
                    getAllSalesOrder.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all SalesOrders");
                var result = _mapper.Map<IEnumerable<SalesOrderDto>>(getAllSalesOrder);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogInfo($"Returned owner with id: {ex.Message}{ex.InnerException}");
                 
                serviceResponse.Data = null;
                serviceResponse.Message = ($"Returned owner with id: {ex.Message}{ex.InnerException}");
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        // GET api/<PurchaseOrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderById(int id)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();
            try
            {
                var salesOrderById = await _repository.GetSalesOrderById(id);

                if (salesOrderById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    SalesOrderDto salesOrderDto = _mapper.Map<SalesOrderDto>(salesOrderById);

                    List<SalesOrderItemsDto> salesOrderItemsDtoList = new List<SalesOrderItemsDto>();
                    var salesAdditionalChargesDto = salesOrderDto.SalesOrderAdditionalCharges;

                    var salesAdditionalChargesList = new List<SalesOrderAdditionalChargesDto>();
                    if (salesAdditionalChargesDto != null)
                    {
                        for (int i = 0; i < salesAdditionalChargesDto.Count; i++)
                        {
                            SalesOrderAdditionalChargesDto additionalChargesDetails = _mapper.Map<SalesOrderAdditionalChargesDto>(salesAdditionalChargesDto[i]);
                            salesAdditionalChargesList.Add(additionalChargesDetails);
                        }
                    }
                    
                    foreach (var salesOrderItemDetails in salesOrderById.SalesOrdersItems)
                    {
                        SalesOrderItemsDto salesOrderItemsDtos = _mapper.Map<SalesOrderItemsDto>(salesOrderItemDetails);
                        salesOrderItemsDtos.ScheduleDates = _mapper.Map<List<ScheduleDateDto>>(salesOrderItemDetails.ScheduleDates);
                        var SalesItemNo = salesOrderItemsDtos.ItemNumber;
                        var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"], "GetStockDetailsForAllLocationWarehouseByItemNo?", "ItemNumber=", SalesItemNo));
                        var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic inventoryObject = inventoryObjectData;
                        // Convert double to decimal
                        decimal availableStock = Convert.ToDecimal(inventoryObject);
                        salesOrderItemsDtos.AvailableStock = availableStock;
 
                        salesOrderItemsDtoList.Add(salesOrderItemsDtos);
                    }

                    salesOrderDto.SalesOrdersItems = salesOrderItemsDtoList;
                    salesOrderDto.SalesOrderAdditionalCharges = salesAdditionalChargesList;
                    serviceResponse.Data = salesOrderDto;
                    serviceResponse.Message = $"Returned SalesOrder with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // POST api/<PurchaseOrderController>
        [HttpPost]
        public async Task<IActionResult> CreateSalesOrder([FromBody] SalesOrderPostDto salesOrderDtoPost)
        {
            ServiceResponse<SalesOrderPostDto> serviceResponse = new ServiceResponse<SalesOrderPostDto>();
            try
            {
                if (salesOrderDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var createSalesOrder = _mapper.Map<SalesOrder>(salesOrderDtoPost);
                var salesOrderItemsDto = salesOrderDtoPost.SalesOrderItemsPostDtos;
                var salesOrderItemsList = new List<SalesOrderItems>();
                var SalesAdditionalChargesList = _mapper.Map<IEnumerable<SalesOrderAdditionalCharges>>(salesOrderDtoPost.SalesOrderAdditionalChargesPostDtos);
                //if (salesOrderItemsDto != null)
                //{
                //    for (int i = 0; i < salesOrderItemsDto.Count; i++)
                //    {
                //        SalesOrdersItems salesOrderItems = _mapper.Map<SalesOrdersItems>(salesOrderItemsDto[i]);
                //        salesOrderItemsList.Add(salesOrderItems);
                //    }
                //}
                //createSalesOrder.SalesOrdersItems = salesOrderItemsList;


                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                //var newcount = await _repository.GetSONumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    createSalesOrder.SalesOrderNumber = days + months + years + "SO" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    createSalesOrder.SalesOrderNumber = days + months + years + "SO" + (e);
                //}

                var dateFormat = days + months + years;
                var soNumber = await _repository.GenerateSONumber();
                createSalesOrder.SalesOrderNumber = dateFormat + soNumber;

                if (salesOrderItemsDto != null)
                {
                    for (int i = 0; i < salesOrderItemsDto.Count; i++)
                    {
                        SalesOrderItems salesOrderItems = _mapper.Map<SalesOrderItems>(salesOrderItemsDto[i]);
                        salesOrderItems.SalesOrderNumber = createSalesOrder.SalesOrderNumber;
                        salesOrderItems.BalanceQty = salesOrderItemsDto[i].OrderQty;
                        salesOrderItemsList.Add(salesOrderItems);
                    }
                }
                createSalesOrder.SalesOrdersItems = salesOrderItemsList;
                createSalesOrder.SalesOrderAdditionalCharges = SalesAdditionalChargesList.ToList();
                await _repository.CreateSalesOrder(createSalesOrder);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " SalesOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //From Date and To Date filter 
        [HttpGet]
        public async Task<IActionResult> SearchSalesOrderDate([FromQuery] SearchDateParam searchDateParam)
        {
            ServiceResponse<IEnumerable<SalesOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderReportDto>>();
            try
            {
                var salesOrderList = await _repository.SearchSalesOrderDate(searchDateParam);

                //_logger.LogInfo("Returned all SalesOrders");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<SalesOrderDto, SalesOrder>().ReverseMap()
                //        .ForMember(dest => dest.SalesOrdersItems, opt => opt.MapFrom(src => src.SalesOrdersItems));
                //});

                //var mapper = config.CreateMapper();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<SalesOrderItems, SalesOrderItemsReportDto>()
                        .ForMember(dest => dest.ScheduleDates, opt => opt.MapFrom(src =>src.ScheduleDates
                        .Select(scheduleDate => new ScheduleDateReportDto
                                {
                                    Id = scheduleDate.Id,
                                    ItemNumber = src.ItemNumber,
                                    SalesOrderNumber = src.SalesOrderNumber,
                                    Date = scheduleDate.Date,
                                    Quantity = scheduleDate.Quantity
                                })
                            )
                        );
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<SalesOrderReportDto>>(salesOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrdersItems";
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
        public async Task<IActionResult> SearchSalesOrder([FromQuery] SearchParammes searchParams)
        {
            ServiceResponse<IEnumerable<SalesOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderReportDto>>();
            try
            {
                var salesOrderList = await _repository.SearchSalesOrder(searchParams);

                _logger.LogInfo("Returned all SalesOrders");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<SalesOrderDto, SalesOrder>().ReverseMap()
                //        .ForMember(dest => dest.SalesOrdersItems, opt => opt.MapFrom(src => src.SalesOrdersItems));
                //});

                //var mapper = config.CreateMapper();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<SalesOrderItems, SalesOrderItemsReportDto>()
                        .ForMember(dest => dest.ScheduleDates, opt => opt.MapFrom(src => src.ScheduleDates
                        .Select(scheduleDate => new ScheduleDateReportDto
                        {
                            Id = scheduleDate.Id,
                            ItemNumber = src.ItemNumber,
                            SalesOrderNumber = src.SalesOrderNumber,
                            Date = scheduleDate.Date,
                            Quantity = scheduleDate.Quantity
                        })
                            )
                        );
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<SalesOrderReportDto>>(salesOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrdersItems";
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
        // sales order item level search

        [HttpGet]
        public async Task<IActionResult> SearchSalesOrderItem([FromQuery] SearchParammes searchParams)
        {
            ServiceResponse<IEnumerable<SalesOrderItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderItemsDto>>();
            try
            {
                var salesOrderList = await _salesOrderItemsRepository.SearchSalesOrderItem(searchParams);
                if (salesOrderList is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrderItem object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrderItem object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var result = _mapper.Map<IEnumerable<SalesOrderItemsDto>>(salesOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrdersItems";
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
        //sales order with items filter concept
        [HttpPost]
        public async Task<IActionResult> GetAllSalesOrderWithItems([FromBody] SalesOrderSearchDto salesOrderSearch)
        {
            ServiceResponse<IEnumerable<SalesOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderReportDto>>();
            try
            {
                var salesOrderList = await _repository.GetAllSalesOrderWithItems(salesOrderSearch);

                _logger.LogInfo("Returned all SalesOrders");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<SalesOrderDto, SalesOrder>().ReverseMap()
                //        .ForMember(dest => dest.SalesOrdersItems, opt => opt.MapFrom(src => src.SalesOrdersItems));
                //});

                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<SalesOrderItems, SalesOrderItemsReportDto>()
                        .ForMember(dest => dest.ScheduleDates, opt => opt.MapFrom(src => src.ScheduleDates
                        .Select(scheduleDate => new ScheduleDateReportDto
                        {
                            Id = scheduleDate.Id,
                            ItemNumber = src.ItemNumber,
                            SalesOrderNumber = src.SalesOrderNumber,
                            Date = scheduleDate.Date,
                            Quantity = scheduleDate.Quantity
                        })
                            )
                        );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<SalesOrderReportDto>>(salesOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrdersItems";
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


        // PUT api/<PurchaseOrderController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalesOrder(int id, [FromBody] SalesOrderUpdateDto salesOrderDtoUpdate)
        {
            ServiceResponse<SalesOrderUpdateDto> serviceResponse = new ServiceResponse<SalesOrderUpdateDto>();
            try
            {
                if (salesOrderDtoUpdate is null)
                {
                    _logger.LogError("Update SalesOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update SalesOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update SalesOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update SalesOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var salesOrderDetail = await _repository.GetSalesOrderById(id);
                if (salesOrderDetail is null)
                {
                    _logger.LogError($"Update SalesOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update SalesOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var salesOrderDetails = _mapper.Map<SalesOrder>(salesOrderDtoUpdate);
                var salesOrderItemsDto = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos;
                var salesAdditionalChargesDto = salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos;
                var salesOrderItemsList = new List<SalesOrderItems>();
                var salesAdditionalChargesList = new List<SalesOrderAdditionalCharges>();
                if(salesAdditionalChargesDto != null)
                {
                    for(int i = 0; i < salesAdditionalChargesDto.Count; i++)
                    {
                        SalesOrderAdditionalCharges additionalChargesDetails = _mapper.Map<SalesOrderAdditionalCharges>(salesAdditionalChargesDto[i]);
                        salesAdditionalChargesList.Add(additionalChargesDetails);
                    }
                }
                if (salesOrderItemsDto != null)
                {
                    for (int i = 0; i < salesOrderItemsDto.Count; i++)
                    {
                        SalesOrderItems salesOrderItemsDetail = _mapper.Map<SalesOrderItems>(salesOrderItemsDto[i]);
                        salesOrderItemsDetail.BalanceQty = salesOrderItemsDetail.OrderQty - salesOrderItemsDetail.DispatchQty;
                        salesOrderItemsList.Add(salesOrderItemsDetail);

                        SalesOrderHistory salesOrderHistory = new SalesOrderHistory();
                        salesOrderHistory.SalesOrderNumber = salesOrderDetail.SalesOrderNumber;
                        salesOrderHistory.ProjectNumber = salesOrderDetail.ProjectNumber;
                        salesOrderHistory.QuoteNumber = salesOrderDetail.QuoteNumber;
                        salesOrderHistory.OrderDate = salesOrderDetail.OrderDate;
                        salesOrderHistory.OrderType = salesOrderDetail.OrderType;
                        salesOrderHistory.CustomerName = salesOrderDetail.CustomerName;
                        salesOrderHistory.CustomerId = salesOrderDetail.CustomerId;
                        salesOrderHistory.RevisionNumber = salesOrderDetail.RevisionNumber;
                        //salesOrderHistory.SOStatus = salesOrderDetail.SOStatus;
                        salesOrderHistory.PONumber = salesOrderDetail.PONumber;
                        salesOrderHistory.PODate = salesOrderDetail.PODate;
                        salesOrderHistory.ReceivedDate = salesOrderDetail.ReceivedDate;
                        salesOrderHistory.BillTo = salesOrderDetail.BillTo;
                        salesOrderHistory.BillToId = salesOrderDetail.BillToId;
                        salesOrderHistory.ShipTo = salesOrderDetail.ShipTo;
                        salesOrderHistory.ShipToId = salesOrderDetail.ShipToId;
                        salesOrderHistory.PaymentTerms = salesOrderDetail.PaymentTerms;
                        salesOrderHistory.Total = salesOrderDetail.Total;
                        salesOrderHistory.Unit = salesOrderDetail.Unit;
                        salesOrderHistory.IsShortClosed = salesOrderDetail.IsShortClosed;
                        salesOrderHistory.ShortClosedBy = salesOrderDetail.ShortClosedBy;
                        salesOrderHistory.ShortClosedOn = salesOrderDetail.ShortClosedOn;
                        salesOrderHistory.CreatedBy = salesOrderDetail.CreatedBy;
                        salesOrderHistory.CreatedOn = salesOrderDetail.CreatedOn;
                        salesOrderHistory.LastModifiedBy = salesOrderDetail.LastModifiedBy;
                        salesOrderHistory.LastModifiedOn = salesOrderDetail.LastModifiedOn;
                        salesOrderHistory.ItemNumber = salesOrderItemsDto[i].ItemNumber;
                        salesOrderHistory.Description = salesOrderItemsDto[i].Description;
                        salesOrderHistory.BalanceQty = salesOrderItemsDto[i].BalanceQty;
                        salesOrderHistory.DispatchQty = salesOrderItemsDto[i].DispatchQty;
                        salesOrderHistory.ShopOrderQty = salesOrderItemsDto[i].ShopOrderQty;
                        salesOrderHistory.UOM = salesOrderItemsDto[i].UOM;
                        salesOrderHistory.Currency = salesOrderItemsDto[i].Currency;
                        salesOrderHistory.TotalAmount = salesOrderItemsDto[i].TotalAmount;
                        salesOrderHistory.BasicAmount = salesOrderItemsDto[i].BasicAmount;
                        salesOrderHistory.Discount = salesOrderItemsDto[i].Discount;
                        salesOrderHistory.UnitPrice = salesOrderItemsDto[i].UnitPrice;
                        salesOrderHistory.OrderQty = salesOrderItemsDto[i].OrderQty;
                        salesOrderHistory.SGST = salesOrderItemsDto[i].SGST;
                        salesOrderHistory.UTGST = salesOrderItemsDto[i].UTGST;
                        salesOrderHistory.CGST = salesOrderItemsDto[i].CGST;
                        salesOrderHistory.IGST = salesOrderItemsDto[i].IGST;
                        salesOrderHistory.ReceivedDate = salesOrderItemsDto[i].RequestedDate;
                        salesOrderHistory.Remarks = salesOrderItemsDto[i].Remarks;


                        var salesOrderHistories = _mapper.Map<SalesOrderHistory>(salesOrderHistory);


                        await _salesOrderHistory.CreateSalesOrderHistory(salesOrderHistories);
                        _salesOrderHistory.SaveAsync();
                    }
                }

                var updateData = _mapper.Map(salesOrderDtoUpdate, salesOrderDetail);
                updateData.SalesOrdersItems = salesOrderItemsList;
                updateData.SalesOrderAdditionalCharges = salesAdditionalChargesList;
                string result = await _repository.UpdateSalesOrder(updateData);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " SalesOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateSalesOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<CompanyMasterController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrder(int id)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();
            try
            {
                var getSalesOrderById = await _repository.GetSalesOrderById(id);
                if (getSalesOrderById == null)
                {
                    _logger.LogError($"Delete SalesOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete SalesOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteSalesOrder(getSalesOrderById);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " SalesOrder Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> ShortCloseShopOrder(int id)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();

            try
            {
                var salesOrderShortCloseById = await _repository.GetSalesOrderById(id);
                if (salesOrderShortCloseById == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                salesOrderShortCloseById.IsShortClosed = true;
                salesOrderShortCloseById.ShortClosedBy = "Admin";
                salesOrderShortCloseById.ShortClosedOn = DateTime.Now;
                string result = await _repository.UpdateSalesOrder(salesOrderShortCloseById);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "SalesOrder have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SalesOrderShortClosed action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet("ItemNo")]
        public async Task<IActionResult> GetprojectNoByItemNo(string itemNo)
        {
            ServiceResponse<IEnumerable<ListOfProjectNoDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfProjectNoDto>>();

            try
            {
                var getProjectByItemNo = await _salesOrderItemsRepository.GetprojectNoByItemNo(itemNo);
                if (getProjectByItemNo.Count() == 0)
                {
                    _logger.LogError($"ProjectNo with id: {itemNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ProjectNo with itemNo, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ProjectNumber with itemNo: {itemNo}");
                    var result = _mapper.Map<List<ListOfProjectNoDto>>(getProjectByItemNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Successfully Returned ListOfProjectNo";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ProjectNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        //get salesorder Detais by CustomerId

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderDetailsByCustomerId(string Customerid)
        {
            ServiceResponse<IEnumerable<ListofSalesOrderDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListofSalesOrderDetails>>();

            try
            {
                var getSalesDetailByCustomerId = await _repository.GetSalesOrderDetailsByCustomerId(Customerid);
                if (getSalesDetailByCustomerId == null)
                {
                    _logger.LogError($"SalesOrderDetail with id: {Customerid}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderDetail with id: {Customerid}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SalesOrderDetail with id: {Customerid}");
                    var result = _mapper.Map<IEnumerable<ListofSalesOrderDetails>>(getSalesDetailByCustomerId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SalesDetail action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderTotalBySalesOrderId(int salesOrderId)
        {
            ServiceResponse<IEnumerable<SalesOrder>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrder>>();

            try
            {
                var salesOrderTotalBySalesOrderId = await _repository.GetSalesOrderTotalBySalesOrderId(salesOrderId);
                if (salesOrderTotalBySalesOrderId == null)
                {
                    _logger.LogError($"SalesOrderDetail with salesOrderId: {salesOrderId}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderDetail with salesOrderId: {salesOrderId}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SalesOrderDetail with salesOrderId: {salesOrderId}");
                    //var result = _mapper.Map<IEnumerable<SalesOrder>>(salesOrderTotalBySalesOrderId);
                    //serviceResponse.Data = result;
                    //serviceResponse.Message = "Success";
                    //serviceResponse.Success = true;
                    //serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(salesOrderTotalBySalesOrderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderDetailsBySalesOrderId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //getprojectnumberbyitemnumber

        //getsalesorderDetailByprojectNoanditemNo --
        [HttpGet]
        public async Task<IActionResult> getSalesOrderDetailByProjectNoandItemNo(string ItemNo, string ProjectNo)
        {
            ServiceResponse<IEnumerable<GetSalesOrderDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<GetSalesOrderDetailsDto>>();

            try
            {
                var getSalesDetail = await _salesOrderItemsRepository.getSalesOrderDetailByProjectNoandItemNo(ItemNo, ProjectNo);
                if (getSalesDetail.Count() == 0)
                {
                    _logger.LogError($"SalesOrderDetail with ItemNo: {ItemNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderDetail with ItemNo: {ItemNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned SalesOrderDetail with ItemNo: {ItemNo}");
                    var result = _mapper.Map<List<GetSalesOrderDetailsDto>>(getSalesDetail);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Successfully Returned SalesOrderDetail";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SalesDetail action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        //pass data from btodeliveryorder using _httpclient warehoouse service to salesservice



        [HttpPost]
        public async Task<IActionResult> UpdateDispatchDetails([FromBody] List<SalesOrderDispatchQtyDto> salesOrderDispatchQtyDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (salesOrderDispatchQtyDto == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from the client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from the client is null.");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrder object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrder object sent from the client.");
                    return BadRequest(serviceResponse);
                }

                foreach (var item in salesOrderDispatchQtyDto)
                {
                    IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.GetSalesOrderDetailsByIdandItemNo(item.FGItemNumber, item.SalesOrderId);
                    var dispatchedQty = item.DispatchQty;

                    foreach (var salesOrderItem in salesOrderItems)
                    {
                        var balanceQty = salesOrderItem.BalanceQty;

                        if (salesOrderItem.BalanceQty >= dispatchedQty)
                        {
                            salesOrderItem.BalanceQty -= dispatchedQty;
                            salesOrderItem.DispatchQty += dispatchedQty;
                            dispatchedQty = 0;
                        }
                        else
                        {
                            salesOrderItem.BalanceQty = 0;
                            salesOrderItem.DispatchQty += balanceQty;
                            dispatchedQty -= balanceQty;
                        }

                        if (salesOrderItem.BalanceQty <= 0)
                        {
                            salesOrderItem.BalanceQty = 0;
                            salesOrderItem.StatusEnum = OrderStatus.Closed;
                        }

                        await _salesOrderItemsRepository.UpdateSalesOrderItem(salesOrderItem);

                        if (dispatchedQty == 0)
                        {
                            break;
                        }
                    }
                }

                _salesOrderItemsRepository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "SalesOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateDispatchDetails action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal error in SalesOrderUpdate";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        //below method old dispatch qty greater then new dispatch qty in bto edit part

        [HttpPost]
        public async Task<IActionResult> UpdateDispatchQtyGreaterThenNewDispatchQty([FromBody] List<SalesOrderDispatchQtyDto> salesOrderDispatchQtyDto)
        {
            //dynamic dispatchDetials
            //we have to write code for same itemnumber in multiple rows
            // Deserialise and store it in dynamic varibale
            //lopp thori=ug the dynamic variable an pass hte item number and so id to salesorderitemdetials, get 
            //the item object change the balanceqty and disoatchqty and pass the data to update method of service.
            foreach (var item in salesOrderDispatchQtyDto)
            {
                IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.GetSalesOrderDetailsByIdandItemNo(item.FGItemNumber, item.SalesOrderId);
                var orderItem = salesOrderItems.FirstOrDefault();
                orderItem.BalanceQty = orderItem.BalanceQty + item.DispatchQty;
                orderItem.DispatchQty -= item.DispatchQty;
                _salesOrderItemsRepository.UpdateSalesOrderItem(orderItem);
            }

            _salesOrderItemsRepository.SaveAsync();
            return Ok();
        }

        //below method old dispatch qty Smaller then new dispatch qty in bto edit part

        [HttpPost]
        public async Task<IActionResult> UpdateDispatchQtySmallerThenNewDispatchQty([FromBody] List<SalesOrderDispatchQtyDto> salesOrderDispatchQtyDto)
        {
            //dynamic dispatchDetials
            //we have to write code for same itemnumber in multiple rows
            // Deserialise and store it in dynamic varibale
            //lopp thori=ug the dynamic variable an pass hte item number and so id to salesorderitemdetials, get 
            //the item object change the balanceqty and disoatchqty and pass the data to update method of service.
            foreach (var item in salesOrderDispatchQtyDto)
            {
                IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.GetSalesOrderDetailsByIdandItemNo(item.FGItemNumber, item.SalesOrderId);
                var orderItem = salesOrderItems.FirstOrDefault();
                orderItem.BalanceQty = orderItem.BalanceQty - item.DispatchQty;
                orderItem.DispatchQty += item.DispatchQty;
                _salesOrderItemsRepository.UpdateSalesOrderItem(orderItem);
            }

            _salesOrderItemsRepository.SaveAsync();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> ReturnDOUpdateDispatchDetails([FromBody] List<ReturnDOSalesOrderDispatchQtyDto> salesOrderDispatchQtyDto)
        {
            //dynamic dispatchDetials
            //we have to write code for same itemnumber in multiple rows
            // Deserialise and store it in dynamic varibale
            //lopp thori=ug the dynamic variable an pass hte item number and so id to salesorderitemdetials, get 
            //the item object change the balanceqty and disoatchqty and pass the data to update method of service.
            foreach (var item in salesOrderDispatchQtyDto)
            {
                IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.GetSalesOrderDetailsByIdandItemNo(item.FGPartNumber, item.SalesOrderId);
                var orderItem = salesOrderItems.FirstOrDefault();
                orderItem.BalanceQty = orderItem.BalanceQty + item.ReturnQty;
                orderItem.DispatchQty -= item.ReturnQty;
                _salesOrderItemsRepository.UpdateSalesOrderItem(orderItem);
            }

            _salesOrderItemsRepository.SaveAsync();
            return Ok();
        }
        //Update Balancre Qty and DispatchQty Using ReturnInvoice 
        [HttpPost]
        public async Task<IActionResult> ReturnInvoiceUpdateDispatchDetails([FromBody] List<ReturnDOSalesOrderDispatchQtyDto> salesOrderDispatchQtyDto)
        {
            foreach (var item in salesOrderDispatchQtyDto)
            {
                IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.GetSalesOrderDetailsByIdandItemNo(item.FGPartNumber, item.SalesOrderId);
                var orderItem = salesOrderItems.FirstOrDefault();
                orderItem.BalanceQty += item.ReturnQty;
                orderItem.DispatchQty -= item.ReturnQty;
                if (orderItem.BalanceQty == orderItem.OrderQty)
                {
                    orderItem.StatusEnum = OrderStatus.Open;
                }
                _salesOrderItemsRepository.UpdateSalesOrderItem(orderItem);
            }

            _salesOrderItemsRepository.SaveAsync();
            return Ok();
        }
        //Update shoporder Qty in salesorder while create shoporderUpdateShopOrderQty
        [HttpPost]
        public async Task<IActionResult> UpdateShopOrderQty([FromBody] ShopOrderReleaseQtyDto shopOrderReleaseQtyDto)
        {
          
                IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.UpdateShopOrderBySalesOrderNoandItemNo(shopOrderReleaseQtyDto.SalesOrderNumber
                                                                                             ,shopOrderReleaseQtyDto.FGItemNumber, shopOrderReleaseQtyDto.ProjectNumber);
                var orderItem = salesOrderItems.FirstOrDefault();

                orderItem.ShopOrderQty += shopOrderReleaseQtyDto.ReleaseQty;
                await _salesOrderItemsRepository.UpdateSalesOrderItem(orderItem);
            
             _salesOrderItemsRepository.SaveAsync();
             return Ok();
        }

        //Update Invoiced Value and DispatchQty Using Invoice 
        [HttpPost]
        public async Task<IActionResult> AdditionalChargeUpdateFromInvoice([FromBody] List<SoAdditionalChargeUpdateDto> soAdditionalChargeUpdateDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (soAdditionalChargeUpdateDto == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from the client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from the client is null.");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrder object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrder object sent from the client.");
                    return BadRequest(serviceResponse);
                }
                foreach (var item in soAdditionalChargeUpdateDto)
                {
                  var salesAdditionalCharges = await _salesAdditionalChargesRepository.GetSalesAdditionalChargesById(item.SalesOrderId,item.SalesAdditionalChargeId);

                    salesAdditionalCharges.InvoicedValue += item.InvoicedValue;
                    await _salesAdditionalChargesRepository.UpdateSalesAdditionalCharges(salesAdditionalCharges);
                    _salesAdditionalChargesRepository.SaveAsync();
                }
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder Successfully Updated";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside InvoiceUpdateDispatchDetails action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal error in SalesOrderUpdate";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        //getsalesorderdetailbyitemnoandsalesorderId

        [HttpGet]
        public async Task<IActionResult> GetFGSalesOrderDetailsByItemNo(string itemNumber)
        {
            ServiceResponse<ItemDetailsForShopOrderDto> serviceResponse = new ServiceResponse<ItemDetailsForShopOrderDto>();
            try
            {
                _logger.LogError(string.Concat(_config["EngineeringBomAPI"], "GetAllProductionBomFGListByItemNumber?", "ItemNumber=", itemNumber));
                var bomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"], "GetAllProductionBomFGListByItemNumber?", "ItemNumber=", itemNumber));
                //_logger.LogError(_httpClient.BaseAddress.ToString());
                //_logger.LogError(_httpClient.ToString());
                var bomDetailsString = await bomDetails.Content.ReadAsStringAsync();
                dynamic bomDetailsStringData = JsonConvert.DeserializeObject(bomDetailsString);
                dynamic bomData = bomDetailsStringData.data;
                string jsonString = JsonConvert.SerializeObject(bomData[0].bomVersionNo);
                JArray jArray = JArray.Parse(jsonString);
                decimal[] bomVersionNo = jArray.ToObject<decimal[]>();
                //List<decimal> bomVersionNo = jArray.ToObject<List<decimal>>();

                ItemDetailsForShopOrderDto itemDetailsDto = new ItemDetailsForShopOrderDto();
                itemDetailsDto.ItemNumber = bomData[0].itemNumber;
                itemDetailsDto.ItemType = bomData[0].itemType;
                itemDetailsDto.BomVersionNo = bomVersionNo;

                var projectSODetails = await _repository.GetProjectDetailsByItemNo(itemNumber);
                foreach (var project in projectSODetails)
                {
                    project.SalesOrderQtyDetails = await _repository.GetSalesOrderQtyDetailsByItemNo(itemNumber, project.ProjectNumber);
                }
                itemDetailsDto.ProjectSODetails = projectSODetails;

                serviceResponse.Data = itemDetailsDto;
                serviceResponse.Message = "Returned all SalesOrderFGDetails";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetFGSalesOrderDetailsByItemNo action {ex.InnerException},{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSASalesOrderDetailsByItemNo(string itemNumber)
        {
            ServiceResponse<SARevisionNumber> serviceResponse = new ServiceResponse<SARevisionNumber>();
            try
            {
                var bomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"], "GetAllProductionBomSAListByItemNumber?", "ItemNumber=", itemNumber));
                var bomDetailsString = await bomDetails.Content.ReadAsStringAsync();
                dynamic bomDetailsStringData = JsonConvert.DeserializeObject(bomDetailsString);
                dynamic bomData = bomDetailsStringData.data;

                string jsonString = JsonConvert.SerializeObject(bomData[0].bomVersionNo);
                JArray jArray = JArray.Parse(jsonString);
                decimal[] bomVersionNo = jArray.ToObject<decimal[]>();
                //List<decimal> bomVersionNo = jArray.ToObject<List<decimal>>();
                //decimal bomVersionNo = decimal.Parse(bomData[0].bomVersionNo.ToString());


                string jString = JsonConvert.SerializeObject(bomData[0].fgItemNumber);
                JArray jsonArray = JArray.Parse(jString);
                List<string> fgItemNumber = jsonArray.ToObject<List<string>>();

                SARevisionNumber itemDetailsDto = new SARevisionNumber();
                itemDetailsDto.ItemNumber = bomData[0].itemNumber;
                itemDetailsDto.FGItemNumber = fgItemNumber;
                itemDetailsDto.ItemType = bomData[0].itemType;
                itemDetailsDto.BomVersionNo = bomVersionNo;

                var projectSODetails = await _repository.GetProjectDetailsByItemNo(itemNumber);
                foreach (var project in projectSODetails)
                { 
                        project.SalesOrderQtyDetails = await _repository.GetSalesOrderQtyDetailsByItemNo(itemNumber, project.ProjectNumber);
                    
                    }
                itemDetailsDto.ProjectSODetails = projectSODetails;

                serviceResponse.Data = itemDetailsDto;
                serviceResponse.Message = "Returned all SalesOrderSADetails";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSASalesOrderDetailsByItemNo action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveSalesOrderIdNameList()
        {
            ServiceResponse<IEnumerable<SalesOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderIdNameListDto>>();
            try
            {
                var listOfActiveSalesOrderName = await _repository.GetAllActiveSalesOrderNameList();

                var result = _mapper.Map<IEnumerable<SalesOrderIdNameListDto>>(listOfActiveSalesOrderName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveSalesOrderIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveSalesOrderIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrderIdNameList()
        {
            ServiceResponse<IEnumerable<SalesOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderIdNameListDto>>();
            try
            {
                var listOfSalesOrderName = await _repository.GetAllSalesOrderIdNameList();

                var result = _mapper.Map<IEnumerable<SalesOrderIdNameListDto>>(listOfSalesOrderName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All SalesOrderIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllSalesOrderIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut]
        public async Task<IActionResult> ActivateSalesOrderApprovalStatus(string salesOrderNumber)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();

            try
            {
                var salesOrderDetails= await _repository.GetSalesOrderDetailsBySONumber(salesOrderNumber);
                if (salesOrderDetails is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrderApprovalStatus object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"SalesOrderApprovalStatus with string: {salesOrderNumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                salesOrderDetails.ApproveStatus = true;
                string result = await _repository.UpdateSalesOrder(salesOrderDetails);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "SalesOrderApprovalStatus Activated Successfully";
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
                _logger.LogError($"Something went wrong inside ActivateSalesOrderApprovalStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActivateSalesOrderConfirmStatus(string salesOrderNumber,DateTime confirmDate)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();

            try
            {
                var salesOrderDetails = await _repository.GetSalesOrderDetailsBySONumber(salesOrderNumber);
                if (salesOrderDetails is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrderConfirmStatus object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"SalesOrderConfirmStatus with string: {salesOrderNumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }

                salesOrderDetails.ConfirmDate = confirmDate;
                salesOrderDetails.ConfirmStatus = true;
                string result = await _repository.UpdateSalesOrder(salesOrderDetails);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "SalesOrderConfirmStatus Activated Successfully";
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
                _logger.LogError($"Something went wrong inside ActivateSalesOrderConfirmStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{itemNumber}")]
        public async Task<ActionResult<decimal>> GetOpenSalesOrderQuantityByItemNumber(string itemNumber)
        {
            var salesOrderServiceResponse = new ServiceResponse<decimal>();

            try
            {
                var openSalesOrderQty = await _repository.GetOpenSalesOrderQuantityByItemNumber(itemNumber);

                salesOrderServiceResponse.Data = openSalesOrderQty;
                salesOrderServiceResponse.Message = "Retrieved open sales order quantity";
                salesOrderServiceResponse.Success = true;

                return Ok(salesOrderServiceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateSalesOrderConfirmStatus action: {ex.Message}");
                salesOrderServiceResponse.Success = false;
                salesOrderServiceResponse.Message = "Error getting open sales order quantity";
                return StatusCode(500, salesOrderServiceResponse);
            }
        }

    }
}





