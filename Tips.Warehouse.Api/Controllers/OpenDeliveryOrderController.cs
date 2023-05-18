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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OpenDeliveryOrderController : ControllerBase
    {
        private IOpenDeliveryOrderRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IInventoryRepository _inventoryRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IOpenDeliveryOrderHistoryRepository _openDeliveryOrderHistoryRepository;


        public OpenDeliveryOrderController(IOpenDeliveryOrderHistoryRepository openDeliveryOrderHistoryRepository, IInventoryTranctionRepository inventoryTranctionRepository,IOpenDeliveryOrderRepository repository, IInventoryRepository inventoryRepository,ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _openDeliveryOrderHistoryRepository = openDeliveryOrderHistoryRepository;


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
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenDeliveryOrders Successfully";
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
                _logger.LogError($"Something went wrong inside GetOpenDeliveryOrdersById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
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
                _logger.LogError($"Something went wrong inside GetOpenDeliveryOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetOpenDeliveryOrderById action: {ex.Message}";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateOpenDeliveryOrder([FromBody] OpenDeliveryOrderDtoPost openDeliveryOrderDtoPost)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
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

                //var newcount = await _repository.GetODONumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    openDeliveryorder.OpenDONumber = days + months + years + "ODO" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    openDeliveryorder.OpenDONumber = days + months + years + "ODO" + (e);
                //}

                var dateFormat = days + months + years;
                var odoNumber = await _repository.GenerateODONumber();
                openDeliveryorder.OpenDONumber = dateFormat + odoNumber;

                if (openDeliveryOrderitemsList != null)
                {

                    for (int i = 0; i < openDeliveryOrderitemsList.Count; i++)
                    {
                        OpenDeliveryOrderParts OpenDeliveryOrderItemsDetails = _mapper.Map<OpenDeliveryOrderParts>(openDeliveryOrderitemsList[i]);
                        OpenDeliveryOrderItemsDetails.ODONumber = openDeliveryorder.OpenDONumber;
                        openDeliveryOrderItemsDtoList.Add(OpenDeliveryOrderItemsDetails);

                        //Update Inventory balanced Quantity 

                        var PartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                        var getInventoryFGDetailsByItemnumber = await _inventoryRepository.GetInventoryFGDetailsByItemNumber(PartNumber);
                        decimal Quantity = Convert.ToDecimal(openDeliveryOrderitemsList[i].DispatchQty);
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
                            inventory.PartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                            inventory.MftrPartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                            inventory.Description = openDeliveryOrderItemsDtoList[i].ItemDescription;
                            inventory.ProjectNumber = "";
                            inventory.Balance_Quantity = openDeliveryOrderItemsDtoList[i].DispatchQty;
                            inventory.UOM = openDeliveryOrderItemsDtoList[i].UOM;
                            inventory.IsStockAvailable = true;
                            inventory.Warehouse = openDeliveryOrderItemsDtoList[i].Warehouse;
                            inventory.Location = openDeliveryOrderItemsDtoList[i].Location;
                            inventory.GrinNo = openDeliveryorder.OpenDONumber;
                            inventory.GrinPartId = 0;
                            inventory.PartType = "";
                            inventory.GrinMaterialType = "";
                            inventory.ReferenceID = openDeliveryorder.OpenDONumber;
                            inventory.ReferenceIDFrom = "Create Open Delivery Order";
                            inventory.shopOrderNo = "";

                            await _inventoryRepository.CreateInventory(inventory);
                            _inventoryRepository.SaveAsync();
                        }


                        //Add BTO Detail Into Inventory transaction Table

                        InventoryTranction inventoryTranction = new InventoryTranction();
                        inventoryTranction.PartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                        inventoryTranction.MftrPartNumber = openDeliveryOrderItemsDtoList[i].ItemNumber;
                        inventoryTranction.Description = openDeliveryOrderItemsDtoList[i].ItemDescription;
                        inventoryTranction.Issued_Quantity = Convert.ToDecimal(openDeliveryOrderItemsDtoList[i].DispatchQty);
                        inventoryTranction.UOM = openDeliveryOrderItemsDtoList[i].UOM;
                        inventoryTranction.Issued_DateTime = DateTime.Now;
                        inventoryTranction.ReferenceID = openDeliveryorder.OpenDONumber;
                        inventoryTranction.ReferenceIDFrom = "Open Delivery Order";
                        inventoryTranction.Issued_By = "Admin";
                        inventoryTranction.CreatedOn = DateTime.Now;
                        inventoryTranction.Unit = "Bangalore";
                        inventoryTranction.CreatedBy = "Admin";
                        inventoryTranction.LastModifiedBy = "Admin";
                        inventoryTranction.LastModifiedOn = DateTime.Now;
                        inventoryTranction.ModifiedStatus = false;
                        inventoryTranction.From_Location = openDeliveryOrderItemsDtoList[i].Location;
                        inventoryTranction.TO_Location = "ODO";
                        inventoryTranction.Remarks = "Create ODO";

                        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                        _inventoryTranctionRepository.SaveAsync();


                        // Add Bto detail in to opendeliveryorderhistory table

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
                        openDeliveryOrderHistory.ODODate = openDeliveryorder.DODate;
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
                        openDeliveryOrderHistory.CreatedBy = openDeliveryOrderItemsDtoList[i].CreatedBy;
                        openDeliveryOrderHistory.LastModifiedOn = openDeliveryOrderItemsDtoList[i].LastModifiedOn;
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
                _logger.LogError($"Something went wrong inside CreateOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                _logger.LogError($"Something went wrong inside UpdateOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                _logger.LogError($"Something went wrong inside DeleteOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenDeliveryOrderIdNameList()
        {
            ServiceResponse<IEnumerable<BtoIDNameList>> serviceResponse = new ServiceResponse<IEnumerable<BtoIDNameList>>();
            try
            {
                var listOfAllOpenDeliveryOrderIdNames = await _repository.GetAllOpenDeliveryOrderIdNameList();
                var result = _mapper.Map<IEnumerable<BtoIDNameList>>(listOfAllOpenDeliveryOrderIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All listOfAllOpenDeliveryOrderIdNames";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllOpenDeliveryOrderIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
    }
}
