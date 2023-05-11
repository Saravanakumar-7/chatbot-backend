using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Repository;

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BTODeliveryOrderController : ControllerBase
    {
        private IBTODeliveryOrderRepository _repository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IBTODeliveryOrderHistoryRepository _bTODeliveryOrderHistoryRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IInventoryRepository _inventoryRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;



        public BTODeliveryOrderController(IBTODeliveryOrderRepository repository, IInventoryTranctionRepository inventoryTranctionRepository , IBTODeliveryOrderHistoryRepository bTODeliveryOrderHistoryRepository, HttpClient httpClient, IConfiguration config, IInventoryRepository inventoryRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _inventoryRepository = inventoryRepository;
            _config = config;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _bTODeliveryOrderHistoryRepository = bTODeliveryOrderHistoryRepository;

        }


        [HttpGet]
        public async Task<IActionResult> GetAllBTODeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderDto>>();
            try
            {
                var getAllBTODeliveryOrdersDetails = await _repository.GetAllBTODeliveryOrders(pagingParameter,searchParams);
                var metadata = new
                {
                    getAllBTODeliveryOrdersDetails.TotalCount,
                    getAllBTODeliveryOrdersDetails.PageSize,
                    getAllBTODeliveryOrdersDetails.CurrentPage,
                    getAllBTODeliveryOrdersDetails.HasNext,
                    getAllBTODeliveryOrdersDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all BTODeliveryOrder");
                var result = _mapper.Map<IEnumerable<BTODeliveryOrderDto>>(getAllBTODeliveryOrdersDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrders";
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

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBTODeliveryOrderById(int id)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var getBTODeliveryOrderDetailById = await _repository.GetBTODeliveryOrderById(id);

                if (getBTODeliveryOrderDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BTODeliveryOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");

                    BTODeliveryOrderDto bTODeliveryOrderDto = _mapper.Map<BTODeliveryOrderDto>(getBTODeliveryOrderDetailById);

                    List<BTODeliveryOrderItemsDto> bTODeliveryOrderItemsDtoList = new List<BTODeliveryOrderItemsDto>();

                    if (getBTODeliveryOrderDetailById.bTODeliveryOrderItems != null)
                    {

                        foreach (var deliveryOrderitemDetails in getBTODeliveryOrderDetailById.bTODeliveryOrderItems)
                        {
                            BTODeliveryOrderItemsDto bTODeliveryOrderItemsDtos = _mapper.Map<BTODeliveryOrderItemsDto>(deliveryOrderitemDetails);
                            //bTODeliveryOrderItemsDtos.BTOSerialNumberDto = _mapper.Map<List<BTOSerialNumberDto>>(deliveryOrderitemDetails.BTOSerialNumbers);
                            bTODeliveryOrderItemsDtoList.Add(bTODeliveryOrderItemsDtos);
                        }
                    }

                    bTODeliveryOrderDto.bTODeliveryOrderItems = bTODeliveryOrderItemsDtoList;

                    serviceResponse.Data = bTODeliveryOrderDto;
                    serviceResponse.Message = "Returned BTODeliveryOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBTODeliveryOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //get bto number list passing customer leadid
        [HttpGet]
        public async Task<IActionResult> SearchBTODeliveryOrderDate([FromQuery] SearchsDateParms searchDateParam)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderDto>>();
            try
            {
                var bTODeliveryOrders = await _repository.SearchBTODeliveryOrderDate(searchDateParam);
                var result = _mapper.Map<IEnumerable<BTODeliveryOrderDto>>(bTODeliveryOrders);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrders";
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
        public async Task<IActionResult> SearchBTODeliveryOrder([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderDto>>();
            try
            {
                var btoDeliveyOrderList = await _repository.SearchBTODeliveryOrder(searchParams);

                _logger.LogInfo("Returned all BTODeliveryOrder");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<BTODeliveryOrder, BTODeliveryOrderDto>().ReverseMap()
                    .ForMember(dest => dest.bTODeliveryOrderItems, opt => opt.MapFrom(src => src.bTODeliveryOrderItems));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<BTODeliveryOrderDto>>(btoDeliveyOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrder";
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
        public async Task<IActionResult> GetAllBTODeliveryOrderWithItems([FromBody] BTODeliveryOrderSearchDto bTODeliveryOrderSearch)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderDto>>();
            try
            {
                var bTODeliveryOrders = await _repository.GetAllBTODeliveryOrderWithItems(bTODeliveryOrderSearch);
                _logger.LogInfo("Returned all bTODeliveryOrders");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<BTODeliveryOrderDto, BTODeliveryOrder>().ReverseMap()
                    .ForMember(dest => dest.bTODeliveryOrderItems, opt => opt.MapFrom(src => src.bTODeliveryOrderItems));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<BTODeliveryOrderDto>>(bTODeliveryOrders);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all bTODeliveryOrders";
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
        public async Task<IActionResult> GetBtoNumberListByCustomerId(string customerLeadId)
        {
            ServiceResponse<IEnumerable<ListOfBtoNumberDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListOfBtoNumberDetails>>();
            try
            {
                var getBTONumberList = await _repository.GetBtoNumberListByCustomerId(customerLeadId);

                if (getBTONumberList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BTODeliveryOrderNumbers not found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BTODeliveryOrderNumbersList with id: {customerLeadId},not found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {customerLeadId}");

                    var bTODeliveryNumberList = _mapper.Map<IEnumerable<ListOfBtoNumberDetails>>(getBTONumberList);
                      

                    serviceResponse.Data = bTODeliveryNumberList;
                    serviceResponse.Message = "Returned BTODeliveryNumberList Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBTODeliveryOrderNumberList action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBTODeliveryOrder([FromBody] BTODeliveryOrderDtoPost bTODeliveryOrderDtoPost)
        {
            ServiceResponse<BTODeliveryOrderDtoPost> serviceResponse = new ServiceResponse<BTODeliveryOrderDtoPost>();
            try
            {
                if (bTODeliveryOrderDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BTODeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("BTODeliveryOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid BTODeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid BTODeliveryOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var bTODeliveryOrder = _mapper.Map<BTODeliveryOrder>(bTODeliveryOrderDtoPost);

                var bTODeliveryOrderitemsList = bTODeliveryOrderDtoPost.BTODeliveryOrderItemsDtoPost;

                var bTODeliveryOrderItemsDtoList = new List<BTODeliveryOrderItems>();

                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                //var newcount = await _repository.GetBTONumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    bTODeliveryOrder.BTONumber = days + months + years + "BTO" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    bTODeliveryOrder.BTONumber = days + months + years + "BTO" + (e);
                //}

                var dateFormat = days + months + years;
                var btoNumber = await _repository.GenerateBTONumber();
                bTODeliveryOrder.BTONumber = dateFormat + btoNumber;

                if (bTODeliveryOrderitemsList != null)
                {
                   
                    for (int i = 0; i < bTODeliveryOrderitemsList.Count; i++)
                    { 
                        BTODeliveryOrderItems bTODeliveryOrderItemsDetails = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderitemsList[i]);
                         bTODeliveryOrderItemsDetails.OrderBalanceQty = bTODeliveryOrderItemsDetails.FGOrderQty - bTODeliveryOrderItemsDetails.DispatchQty;
                        bTODeliveryOrderItemsDetails.BalanceDoQty = bTODeliveryOrderItemsDetails.DispatchQty;
                        bTODeliveryOrderItemsDetails.BTONumber = bTODeliveryOrder.BTONumber;
                        bTODeliveryOrderItemsDtoList.Add(bTODeliveryOrderItemsDetails);

                        //Update Inventory balanced Quantity 

                        var PartNumber = bTODeliveryOrderItemsDtoList[i].FGItemNumber;
                        var getInventoryFGDetailsByItemnumber = await _inventoryRepository.GetInventoryFGDetailsByItemNumber(PartNumber);
                        decimal Quantity = Convert.ToDecimal(bTODeliveryOrderitemsList[i].DispatchQty);
                        if (getInventoryFGDetailsByItemnumber != null)
                        {
                            if (Quantity != 0 && getInventoryFGDetailsByItemnumber.Balance_Quantity >= Quantity)
                            {
                                getInventoryFGDetailsByItemnumber.Balance_Quantity = getInventoryFGDetailsByItemnumber.Balance_Quantity - Quantity;
                                Quantity = 0;
                                if (getInventoryFGDetailsByItemnumber.Balance_Quantity == 0)
                                {
                                    getInventoryFGDetailsByItemnumber.IsStockAvailable = false;
                                }
                            }
                            if (Quantity != 0 && getInventoryFGDetailsByItemnumber.Balance_Quantity < Quantity)
                            {
                                Quantity = Quantity - getInventoryFGDetailsByItemnumber.Balance_Quantity;
                                getInventoryFGDetailsByItemnumber.Balance_Quantity = 0;
                                getInventoryFGDetailsByItemnumber.IsStockAvailable = false;
                            }

                            _inventoryRepository.Update(getInventoryFGDetailsByItemnumber);
                            _inventoryRepository.SaveAsync();
                        }
                        else
                        {
                            Inventory inventory = new Inventory();
                            inventory.PartNumber = bTODeliveryOrderItemsDtoList[i].FGItemNumber;
                            inventory.MftrPartNumber = bTODeliveryOrderItemsDtoList[i].FGItemNumber;
                            inventory.Description = bTODeliveryOrderItemsDtoList[i].Description;
                            inventory.ProjectNumber = "";
                            inventory.Balance_Quantity = bTODeliveryOrderitemsList[i].DispatchQty;
                            inventory.UOM = bTODeliveryOrderItemsDtoList[i].UOM;
                            inventory.IsStockAvailable = true;
                            inventory.Warehouse = "FG";
                            inventory.Location = "FG";
                            inventory.GrinNo = bTODeliveryOrder.BTONumber;
                            inventory.GrinPartId = 0;
                            inventory.PartType = "";
                            inventory.GrinMaterialType = "";
                            inventory.ReferenceID = bTODeliveryOrder.BTONumber;
                            inventory.ReferenceIDFrom = "Create BTO Delivery Order";
                            inventory.shopOrderNo = "";

                            await _inventoryRepository.CreateInventory(inventory);
                            _inventoryRepository.SaveAsync();
                        }
                        

                        //Add BTO Detail Into Inventory transaction Table

                        InventoryTranction inventoryTranction = new InventoryTranction();
                        inventoryTranction.PartNumber = bTODeliveryOrderItemsDtoList[i].FGItemNumber;
                        inventoryTranction.MftrPartNumber = bTODeliveryOrderItemsDtoList[i].FGItemNumber;
                        inventoryTranction.Description = bTODeliveryOrderItemsDtoList[i].Description;
                        inventoryTranction.Issued_Quantity = Convert.ToDecimal(bTODeliveryOrderItemsDtoList[i].DispatchQty);
                        inventoryTranction.UOM = bTODeliveryOrderItemsDtoList[i].UOM;
                        inventoryTranction.Issued_DateTime = DateTime.Now;
                        inventoryTranction.ReferenceID = bTODeliveryOrder.BTONumber;
                        inventoryTranction.ReferenceIDFrom = "BTO Delivery Order";
                        inventoryTranction.Issued_By = "Admin";
                        inventoryTranction.CreatedOn = DateTime.Now;
                        inventoryTranction.Unit = "Bangalore"; 
                        inventoryTranction.CreatedBy = "Admin";
                        inventoryTranction.LastModifiedBy = "Admin";
                        inventoryTranction.LastModifiedOn = DateTime.Now; 
                        inventoryTranction.ModifiedStatus = false;
                        inventoryTranction.From_Location = "FG";
                        inventoryTranction.TO_Location = "BTO";
                        inventoryTranction.Remarks = "Create BTO";

                        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                        _inventoryTranctionRepository.SaveAsync();


                        // Add Bto detail in to btodeliveryorderhistory table

                        BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                        bTODeliveryOrderHistory.BTONumber = bTODeliveryOrder.BTONumber;
                        bTODeliveryOrderHistory.CustomerName = bTODeliveryOrder.CustomerName;
                        bTODeliveryOrderHistory.CustomerAliasName = bTODeliveryOrder.CustomerAliasName;
                        bTODeliveryOrderHistory.CustomerId = bTODeliveryOrder.CustomerId;
                        bTODeliveryOrderHistory.PONumber = bTODeliveryOrder.PONumber;
                        bTODeliveryOrderHistory.IssuedTo = bTODeliveryOrder.IssuedTo;
                        bTODeliveryOrderHistory.DODate = bTODeliveryOrder.DODate;
                        bTODeliveryOrderHistory.FGItemNumber = bTODeliveryOrderItemsDtoList[i].FGItemNumber;
                        bTODeliveryOrderHistory.SalesOrderId = bTODeliveryOrderItemsDtoList[i].SalesOrderId;
                        bTODeliveryOrderHistory.Description = bTODeliveryOrderItemsDtoList[i].Description;
                        bTODeliveryOrderHistory.BalanceDoQty = bTODeliveryOrderItemsDtoList[i].BalanceDoQty;
                        bTODeliveryOrderHistory.UnitPrice = bTODeliveryOrderItemsDtoList[i].UnitPrice;
                        bTODeliveryOrderHistory.UOC = bTODeliveryOrderItemsDtoList[i].UOC;
                        bTODeliveryOrderHistory.UOM = bTODeliveryOrderItemsDtoList[i].UOM;
                        bTODeliveryOrderHistory.FGOrderQty = bTODeliveryOrderItemsDtoList[i].FGOrderQty;
                        bTODeliveryOrderHistory.OrderBalanceQty = bTODeliveryOrderItemsDtoList[i].OrderBalanceQty;
                        bTODeliveryOrderHistory.FGStock = bTODeliveryOrderItemsDtoList[i].FGStock;
                        bTODeliveryOrderHistory.Discount = bTODeliveryOrderItemsDtoList[i].Discount;
                        bTODeliveryOrderHistory.NetValue = bTODeliveryOrderItemsDtoList[i].NetValue;
                        bTODeliveryOrderHistory.DispatchQty = bTODeliveryOrderItemsDtoList[i].DispatchQty;
                        bTODeliveryOrderHistory.InvoicedQty = bTODeliveryOrderItemsDtoList[i].InvoicedQty;
                        bTODeliveryOrderHistory.SerialNo = bTODeliveryOrderItemsDtoList[i].SerialNo;
                        bTODeliveryOrderHistory.CreatedBy = bTODeliveryOrderItemsDtoList[i].CreatedBy;
                        bTODeliveryOrderHistory.LastModifiedOn = bTODeliveryOrderItemsDtoList[i].LastModifiedOn;
                        bTODeliveryOrderHistory.Remark = "From Create BTO";


                        var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);
                         

                        await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);
                        _bTODeliveryOrderHistoryRepository.SaveAsync();

                    }
                }

                bTODeliveryOrder.bTODeliveryOrderItems = bTODeliveryOrderItemsDtoList;

                await _repository.CreateBTODeliveryOrder(bTODeliveryOrder);
                _repository.SaveAsync();




                //update balance qty and dispatch qty in salesorder table
                var btoDeliveryDispatchDetails = _mapper.Map<List<BtoDeliveryOrderDispatchQtyDetailsDto>>(bTODeliveryOrderitemsList);

                var json = JsonConvert.SerializeObject(btoDeliveryDispatchDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "UpdateDispatchDetails"), data);


                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateBTODelivaryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBTODeliveryOrder(int id, [FromBody] BTODeliveryOrderDtoUpdate bTODeliveryOrderDtoUpdate)
        {
            ServiceResponse<BTODeliveryOrderDtoUpdate> serviceResponse = new ServiceResponse<BTODeliveryOrderDtoUpdate>();
            try
            {
                if (bTODeliveryOrderDtoUpdate is null)
                {
                    _logger.LogError("Update BTODeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update BTODeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update BTODeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update BTODeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var bTODeliveryOrderbyId = await _repository.GetBTODeliveryOrderById(id);
                if (bTODeliveryOrderbyId is null)
                {
                    _logger.LogError($"Update BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update BTODeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                 

                var bTODeliveryOrder = _mapper.Map<BTODeliveryOrder>(bTODeliveryOrderbyId);

                var getOldbtoDeliveryOrderItemsDetails = bTODeliveryOrder.bTODeliveryOrderItems;

                var bTODeliveryOrderitemsDto = bTODeliveryOrderDtoUpdate.BTODeliveryOrderItemsDtoUpdate;
                var bTODeliveryOrderitemsList = new List<BTODeliveryOrderItems>();

                if (bTODeliveryOrderitemsDto != null)
                {
                    for (int j = 0; j < getOldbtoDeliveryOrderItemsDetails.Count; j++)
                    { 
                        for (int i = 0; i < bTODeliveryOrderitemsDto.Count; i++)
                        {
                            BTODeliveryOrderItems bTODeliveryOrderItems = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderitemsDto[i]);
                           // bTODeliveryOrderItems.BTOSerialNumbers = _mapper.Map<List<BTOSerialNumber>>(bTODeliveryOrderitemsDto[i].BTOSerialNumberDtoUpdate);
                            bTODeliveryOrderitemsList.Add(bTODeliveryOrderItems);

                            //Update Inventory balanced Quantity 

                            //var PartNumber = bTODeliveryOrderitemsDto[i].FGItemNumber;
                            //var getInventoryDetails = await _inventoryRepository.GetInventoryDetails(PartNumber);
                            //decimal Quantity = Convert.ToDecimal(bTODeliveryOrderitemsDto[i].DispatchQty);
                            //if (getInventoryDetails != null)
                            //{
                            //    if (getOldbtoDeliveryOrderItemsDetails[j].DispatchQty > Quantity && Quantity !=0)
                            //    {
                            //        var diff = getOldbtoDeliveryOrderItemsDetails[j].DispatchQty - Quantity;
                                   
                            //            getInventoryDetails.Balance_Quantity = getInventoryDetails.Balance_Quantity + diff;
                                        
                            //            if (getInventoryDetails.Balance_Quantity != 0)
                            //            {
                            //                getInventoryDetails.IsStockAvailable = true;
                            //            }

                            //        //Add BTO Detail Into Inventory transaction Table

                            //        InventoryTranction inventoryTranction = new InventoryTranction();
                            //        inventoryTranction.PartNumber = bTODeliveryOrderitemsDto[i].FGItemNumber;
                            //        inventoryTranction.MftrPartNumber = bTODeliveryOrderitemsDto[i].FGItemNumber;
                            //        inventoryTranction.Description = bTODeliveryOrderitemsDto[i].Description;
                            //        inventoryTranction.Issued_Quantity = diff;
                            //        inventoryTranction.UOM = bTODeliveryOrderitemsDto[i].UOM;
                            //        inventoryTranction.Issued_DateTime = DateTime.Now;
                            //        inventoryTranction.ReferenceID = bTODeliveryOrder.BTONumber;
                            //        inventoryTranction.ReferenceIDFrom = "Update BTO Delivery Order";
                            //        inventoryTranction.Issued_By = "Admin";
                            //        inventoryTranction.CreatedOn = DateTime.Now;
                            //        inventoryTranction.Unit = "Bangalore";
                            //        inventoryTranction.CreatedBy = "Admin";
                            //        inventoryTranction.LastModifiedBy = "Admin";
                            //        inventoryTranction.LastModifiedOn = DateTime.Now;
                            //        inventoryTranction.ModifiedStatus = false;
                            //        inventoryTranction.From_Location = "FG";
                            //        inventoryTranction.TO_Location = "BTO";
                            //        inventoryTranction.Remarks = "Update,BTO";

                            //        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                            //        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                            //        _inventoryTranctionRepository.SaveAsync();
                            //    }
                            //    if (getOldbtoDeliveryOrderItemsDetails[j].DispatchQty < Quantity && Quantity != 0)
                            //    {
                            //        var diff = Quantity - getOldbtoDeliveryOrderItemsDetails[j].DispatchQty;
                            //        if (getInventoryDetails.Balance_Quantity >= diff)
                            //        {
                            //            getInventoryDetails.Balance_Quantity = getInventoryDetails.Balance_Quantity - diff;
                                        
                            //            if (getInventoryDetails.Balance_Quantity == 0)
                            //            {
                            //                getInventoryDetails.IsStockAvailable = false;
                            //            }
                            //        }

                            //        //Add BTO Detail Into Inventory transaction Table

                            //        InventoryTranction inventoryTranction = new InventoryTranction();
                            //        inventoryTranction.PartNumber = bTODeliveryOrderitemsList[i].FGItemNumber;
                            //        inventoryTranction.MftrPartNumber = bTODeliveryOrderitemsList[i].FGItemNumber;
                            //        inventoryTranction.Description = bTODeliveryOrderitemsList[i].Description;
                            //        inventoryTranction.Issued_Quantity = diff;
                            //        inventoryTranction.UOM = bTODeliveryOrderitemsList[i].UOM;
                            //        inventoryTranction.Issued_DateTime = DateTime.Now;
                            //        inventoryTranction.ReferenceID = bTODeliveryOrder.BTONumber;
                            //        inventoryTranction.ReferenceIDFrom = "Update BTO Delivery Order";
                            //        inventoryTranction.Issued_By = "Admin";
                            //        inventoryTranction.CreatedOn = DateTime.Now;
                            //        inventoryTranction.Unit = "Bangalore";
                            //        inventoryTranction.CreatedBy = "Admin";
                            //        inventoryTranction.LastModifiedBy = "Admin";
                            //        inventoryTranction.LastModifiedOn = DateTime.Now;
                            //        inventoryTranction.ModifiedStatus = false;
                            //        inventoryTranction.From_Location = "FG";
                            //        inventoryTranction.TO_Location = "BTO";
                            //        inventoryTranction.Remarks = "Update,BTO";

                            //        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                            //        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                            //        _inventoryTranctionRepository.SaveAsync();
                            //    }                                 

                            //}
                            //_inventoryRepository.Update(getInventoryDetails);
                            //_inventoryRepository.SaveAsync();

                            ////update dispatch qty and balance qty in sales order 

                            //if (getOldbtoDeliveryOrderItemsDetails[j].DispatchQty > Quantity && Quantity != 0)
                            //{
                            //    var diff = getOldbtoDeliveryOrderItemsDetails[j].DispatchQty - Quantity;
                            //    Quantity = 0;
                            //    bTODeliveryOrderitemsDto[i].DispatchQty = diff;
                            //    var btoDeliveryDispatchDetails = _mapper.Map<List<BtoDeliveryOrderDispatchQtyDetailsDto>>(bTODeliveryOrderitemsDto);
                                
                            //    var json = JsonConvert.SerializeObject(btoDeliveryDispatchDetails);
                            //    var data = new StringContent(json, Encoding.UTF8, "application/json");
                            //    var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "UpdateDispatchQtyGreaterThenNewDispatchQty"), data);
                            //}
                             

                            //if (getOldbtoDeliveryOrderItemsDetails[j].DispatchQty < Quantity && Quantity != 0)
                            //{
                            //    var diff = Quantity - getOldbtoDeliveryOrderItemsDetails[j].DispatchQty;
                            //    Quantity = 0;
                            //    bTODeliveryOrderitemsDto[i].DispatchQty = diff;
                            //    var btoDeliveryDispatchDetails = _mapper.Map<List<BtoDeliveryOrderDispatchQtyDetailsDto>>(bTODeliveryOrderitemsDto);
                                
                            //    var json = JsonConvert.SerializeObject(btoDeliveryDispatchDetails);
                            //    var data = new StringContent(json, Encoding.UTF8, "application/json");
                            //    var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "UpdateDispatchQtySmallerThenNewDispatchQty"), data);
                            //}

                            //// Add Bto detail in to btodeliveryorderhistory table

                            //BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                            //bTODeliveryOrderHistory.BTONumber = bTODeliveryOrder.BTONumber;
                            //bTODeliveryOrderHistory.CustomerName = bTODeliveryOrder.CustomerName;
                            //bTODeliveryOrderHistory.CustomerAliasName = bTODeliveryOrder.CustomerAliasName;
                            //bTODeliveryOrderHistory.CustomerId = bTODeliveryOrder.CustomerId;
                            //bTODeliveryOrderHistory.PONumber = bTODeliveryOrder.PONumber;
                            //bTODeliveryOrderHistory.IssuedTo = bTODeliveryOrder.IssuedTo;
                            //bTODeliveryOrderHistory.DODate = bTODeliveryOrder.DODate;
                            //bTODeliveryOrderHistory.FGItemNumber = bTODeliveryOrderitemsList[i].FGItemNumber;
                            //bTODeliveryOrderHistory.SalesOrderId = bTODeliveryOrderitemsList[i].SalesOrderId;
                            //bTODeliveryOrderHistory.Description = bTODeliveryOrderitemsList[i].Description;
                            //bTODeliveryOrderHistory.BalanceDoQty = bTODeliveryOrderitemsList[i].BalanceDoQty;
                            //bTODeliveryOrderHistory.UnitPrice = bTODeliveryOrderitemsList[i].UnitPrice;
                            //bTODeliveryOrderHistory.UOC = bTODeliveryOrderitemsList[i].UOC;
                            //bTODeliveryOrderHistory.UOM = bTODeliveryOrderitemsList[i].UOM;
                            //bTODeliveryOrderHistory.FGOrderQty = bTODeliveryOrderitemsList[i].FGOrderQty;
                            //bTODeliveryOrderHistory.OrderBalanceQty = bTODeliveryOrderitemsList[i].OrderBalanceQty;
                            //bTODeliveryOrderHistory.FGStock = bTODeliveryOrderitemsList[i].FGStock;
                            //bTODeliveryOrderHistory.Discount = bTODeliveryOrderitemsList[i].Discount;
                            //bTODeliveryOrderHistory.NetValue = bTODeliveryOrderitemsList[i].NetValue;
                            //bTODeliveryOrderHistory.DispatchQty = bTODeliveryOrderitemsList[i].DispatchQty;
                            //bTODeliveryOrderHistory.InvoicedQty = bTODeliveryOrderitemsList[i].InvoicedQty;
                            //bTODeliveryOrderHistory.SerialNo = bTODeliveryOrderitemsList[i].SerialNo;
                            //bTODeliveryOrderHistory.CreatedBy = bTODeliveryOrderitemsList[i].CreatedBy;
                            //bTODeliveryOrderHistory.LastModifiedOn = bTODeliveryOrderitemsList[i].LastModifiedOn;
                            //bTODeliveryOrderHistory.Remark = "From Update BTO";


                            //var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);


                            //await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);
                            //_bTODeliveryOrderHistoryRepository.SaveAsync();
                        }
                    }
                }

                bTODeliveryOrder.bTODeliveryOrderItems = bTODeliveryOrderitemsList;
                var updateBTODeliveryOrder = _mapper.Map(bTODeliveryOrderDtoUpdate, bTODeliveryOrder);

                string result = await _repository.UpdateBTODeliveryOrder(updateBTODeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateBTODeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBTODeliveryOrder(int id)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var deleteBTODeliveryOrder = await _repository.GetBTODeliveryOrderById(id);
                if (deleteBTODeliveryOrder == null)
                {
                    _logger.LogError($"Delete BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete BTODeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteBTODeliveryOrder(deleteBTODeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteBTODeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetBtoDeliveryOrderNumberList()
        {
            ServiceResponse<IEnumerable<ListofBtoDeliveryOrderDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListofBtoDeliveryOrderDetails>>();

            try
            {
                var getBtoDeliveryOrderNumberList = await _repository.GetBtoDeliveryOrderNumberList();
                if (getBtoDeliveryOrderNumberList == null)
                {
                    _logger.LogError("BtoDeliveryOrderDetail Not Found");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BtoDeliveryOrderDetail Not Found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo("Return BtoDeliveryOrderDetail");
                    var result = _mapper.Map<IEnumerable<ListofBtoDeliveryOrderDetails>>(getBtoDeliveryOrderNumberList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong BtoDeliveryOrder Details: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetInventoryListByItemNo(string ItemNumber)
        {
            ServiceResponse<IEnumerable<GetInventoryListByItemNo>> serviceResponse = new ServiceResponse<IEnumerable<GetInventoryListByItemNo>>();

            try
            {
                var getInventoryListByItemNo = await _inventoryRepository.GetInventoryListByItemNo(ItemNumber);
                if (getInventoryListByItemNo == null)
                {
                    _logger.LogError($"InventoryDetails with id: {ItemNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryDetails with id: {ItemNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned InventoryDetails with id: {ItemNumber}");
                    var result = _mapper.Map<IEnumerable<GetInventoryListByItemNo>>(getInventoryListByItemNo);
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
        public async Task<IActionResult> GetAllBTOIdNameIdNameList()
        {
            ServiceResponse<IEnumerable<BtoIDNameList>> serviceResponse = new ServiceResponse<IEnumerable<BtoIDNameList>>();
            try
            {
                var listOfAllBtoIdNames = await _repository.GetAllBTOIdNameIdNameList();
                var result = _mapper.Map<IEnumerable<BtoIDNameList>>(listOfAllBtoIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All GetAllBTOIdNameIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllBTOIdNameIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }

}
