using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
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
    //[Authorize]
    public class ReturnBtoDeliveryOrderController : ControllerBase
    {
        private IReturnBtoDeliveryOrderRepository _repository;
        private IInventoryRepository _inventoryRepository;
        private IBTODeliveryOrderHistoryRepository _bTODeliveryOrderHistoryRepository;
        private IBTODeliveryOrderItemsRepository _bTODeliveryOrderItemsRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;

        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ReturnBtoDeliveryOrderController(IReturnBtoDeliveryOrderRepository repository, IInventoryTranctionRepository inventoryTranctionRepository, IBTODeliveryOrderHistoryRepository bTODeliveryOrderHistoryRepository, IBTODeliveryOrderItemsRepository bTODeliveryOrderItemsRepository, IInventoryRepository inventoryRepository, HttpClient httpClient, IConfiguration config, ILoggerManager logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _inventoryRepository = inventoryRepository;
            _bTODeliveryOrderItemsRepository = bTODeliveryOrderItemsRepository;
            _bTODeliveryOrderHistoryRepository = bTODeliveryOrderHistoryRepository;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReturnDeliveryOrder([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParams)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderHistory>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderHistory>>();
            try
            {
                var btoHistoryDetails = await _bTODeliveryOrderHistoryRepository.GetAllReturnDeliveryOrder(pagingParameter, searchParams);
                var metadata = new
                {
                    btoHistoryDetails.TotalCount,
                    btoHistoryDetails.PageSize,
                    btoHistoryDetails.CurrentPage,
                    btoHistoryDetails.HasNext,
                    btoHistoryDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all BTODeliveryOrderHistories");
                var result = _mapper.Map<IEnumerable<BTODeliveryOrderHistory>>(btoHistoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrderHistories";
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
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> ReturnDeliveryOrderSPReportDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<ReturnDOSPReport>> serviceResponse = new ServiceResponse<IEnumerable<ReturnDOSPReport>>();
            try
            {
                var products = await _repository.ReturnDeliveryOrderSPReportDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ReturnDeliveryOrderSPReportDate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ReturnDeliveryOrderSPReportDate hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ReturnDeliveryOrderSPReportDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside Invoice action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> ReturnDOSPReportWithParam([FromBody] ReturnDOSPReportWithParamDTO returnDOSPReportDTO)
        {
            ServiceResponse<IEnumerable<ReturnDOSPReport>> serviceResponse = new ServiceResponse<IEnumerable<ReturnDOSPReport>>();
            try
            {
                var products = await _repository.ReturnDOSPReportWithParam(returnDOSPReportDTO.ReturnBTONumber, returnDOSPReportDTO.CustomerName, returnDOSPReportDTO.CustomerAliasName, 
                                                        returnDOSPReportDTO.CustomerLeadId, returnDOSPReportDTO.SalesOrderNumber, returnDOSPReportDTO.ProductType, 
                                                        returnDOSPReportDTO.TypeOfSolution,  returnDOSPReportDTO.Warehouse, returnDOSPReportDTO.Location, 
                                                        returnDOSPReportDTO.KPN, returnDOSPReportDTO.MPN);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ReturnDOSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ReturnDOSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ReturnDOSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside ReturnDOSPReport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReturnDeliveryOrderSPReport([FromQuery] PagingParameter pagingParameter)
        {

            ServiceResponse<IEnumerable<ReturnDOSPReport>> serviceResponse = new ServiceResponse<IEnumerable<ReturnDOSPReport>>();

            try
            {
                var products = await _repository.ReturnDeliveryOrderSPReport(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all ReturnDOSPReport");
                var result = _mapper.Map<IEnumerable<ReturnDOSPReport>>(products);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ReturnDOSPReport Successfully";
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
        public async Task<IActionResult> GetAllBtoHistoryDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParams)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderHistory>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderHistory>>();
            try
            {
                var btoHistoryDetails = await _bTODeliveryOrderHistoryRepository.GetAllBtoHistoryDetails(pagingParameter, searchParams);
                var metadata = new
                {
                    btoHistoryDetails.TotalCount,
                    btoHistoryDetails.PageSize,
                    btoHistoryDetails.CurrentPage,
                    btoHistoryDetails.HasNext,
                    btoHistoryDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all BTODeliveryOrderHistories");
                var result = _mapper.Map<IEnumerable<BTODeliveryOrderHistory>>(btoHistoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrderHistories";
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

        //[HttpGet]
        //public async Task<IActionResult> GetAllBtoHistoryDetails([FromQuery] PagingParameter pagingParameter)
        //{
        //    ServiceResponse<IEnumerable<BTODeliveryOrderHistory>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderHistory>>();
        //    try
        //    {
        //        var btoHistoryDetails = await _bTODeliveryOrderHistoryRepository.GetAllBtoHistoryDetails(pagingParameter);
        //        var metadata = new
        //        {
        //            btoHistoryDetails.TotalCount,
        //            btoHistoryDetails.PageSize,
        //            btoHistoryDetails.CurrentPage,
        //            btoHistoryDetails.HasNext,
        //            btoHistoryDetails.HasPreviuos
        //        };

        //        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

        //        _logger.LogInfo("Returned all BTODeliveryOrderHistories");
        //        var result = _mapper.Map<IEnumerable<BTODeliveryOrderHistory>>(btoHistoryDetails);                
        //        serviceResponse.Data = result;
        //        serviceResponse.Message = "Returned all BTODeliveryOrderHistories";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong,try again";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllReturnBtoDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<ReturnBtoDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnBtoDeliveryOrderDto>>();
            try
            {
                var returnBtoDetails = await _repository.GetAllReturnBtoDeliveryOrderDetails(pagingParameter, searchParams);
                var metadata = new
                {
                    returnBtoDetails.TotalCount,
                    returnBtoDetails.PageSize,
                    returnBtoDetails.CurrentPage,
                    returnBtoDetails.HasNext,
                    returnBtoDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ReturnBTODeliveryOrders");
                var result = _mapper.Map<IEnumerable<ReturnBtoDeliveryOrderDto>>(returnBtoDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ReturnBTODeliveryOrders";
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
        public async Task<IActionResult> GetBtoHistoryDetailsById(int id)
        {
            ServiceResponse<BTODeliveryOrderHistory> serviceResponse = new ServiceResponse<BTODeliveryOrderHistory>();

            try
            {
                var btoHistoryDetailById = await _bTODeliveryOrderHistoryRepository.GetBtoHistoryDetailsById(id);
                if (btoHistoryDetailById == null)
                {
                    _logger.LogError($"BtoHistoryDetail with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BtoHistoryDetail with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned BtoHistoryDetail with id: {id}");
                    var result = _mapper.Map<BTODeliveryOrderHistory>(btoHistoryDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned BtoHistoryDetail Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBtoHistoryDetailsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{btoNumber}")]
        public async Task<IActionResult> GetBtoHistoryDetailsByBtoNo(string btoNumber, string uniqueId)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderHistory>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderHistory>>();

            try
            {
                var btoHistoryDetailByBtoNo = await _bTODeliveryOrderHistoryRepository.GetBtoHistoryDetailsByBtoNo(btoNumber, uniqueId);
                if (btoHistoryDetailByBtoNo == null)
                {
                    _logger.LogError($"BtoHistoryDetail with id: {btoNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BtoHistoryDetail with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned BtoHistoryDetail with id: {btoNumber}");
                    var result = _mapper.Map<IEnumerable<BTODeliveryOrderHistory>>(btoHistoryDetailByBtoNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned BtoHistoryDetail Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBtoHistoryDetailsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetReturnBtoDeliveryOrderById(int id)
        {
            ServiceResponse<ReturnBtoDeliveryOrderDto> serviceResponse = new ServiceResponse<ReturnBtoDeliveryOrderDto>();
            try
            {
                var getReturnBtoDeliveryOrderDetailById = await _repository.GetReturnBtoDeliveryOrderById(id);

                if (getReturnBtoDeliveryOrderDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ReturnBTODeliveryOrder  hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ReturnBTODeliveryOrder with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ReturnBTODeliveryOrder with id: {id}");

                    ReturnBtoDeliveryOrderDto returnBtoDeliveryOrderDto = _mapper.Map<ReturnBtoDeliveryOrderDto>(getReturnBtoDeliveryOrderDetailById);

                    List<ReturnBtoDeliveryOrderItemsDto> returnBtoDeliveryOrderItemsDtoList = new List<ReturnBtoDeliveryOrderItemsDto>();

                    if (getReturnBtoDeliveryOrderDetailById.ReturnBtoDeliveryOrderItems != null)
                    {

                        foreach (var btoDeliveryOrderitemDetails in getReturnBtoDeliveryOrderDetailById.ReturnBtoDeliveryOrderItems)
                        {
                            ReturnBtoDeliveryOrderItemsDto returnBtoDeliveryOrderItemsDtos = _mapper.Map<ReturnBtoDeliveryOrderItemsDto>(btoDeliveryOrderitemDetails);
                            returnBtoDeliveryOrderItemsDtos.QtyDistribution = _mapper.Map<List<ReturnDeliveryOrderItemQtyDistributionDto>>(btoDeliveryOrderitemDetails.QtyDistribution);
                            returnBtoDeliveryOrderItemsDtoList.Add(returnBtoDeliveryOrderItemsDtos);
                        }
                    }

                    returnBtoDeliveryOrderDto.ReturnBtoDeliveryOrderItemsDtos = returnBtoDeliveryOrderItemsDtoList;

                    serviceResponse.Data = returnBtoDeliveryOrderDto;
                    serviceResponse.Message = "Returned ReturnBtoDeliveryOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetReturnBtoDeliveryOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateReturnBtoDeliveryOrder([FromBody] ReturnBtoDeliveryOrderPostDto returnBtoDeliveryOrderPostDto)
        {
            ServiceResponse<ReturnBtoDeliveryOrderPostDto> serviceResponse = new ServiceResponse<ReturnBtoDeliveryOrderPostDto>();
            try
            {
                if (returnBtoDeliveryOrderPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReturnBtoDeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ReturnBtoDeliveryOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ReturnBtoDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ReturnBtoDeliveryOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }


                var returnBtoDeliveryOrder = _mapper.Map<ReturnBtoDeliveryOrder>(returnBtoDeliveryOrderPostDto);

                var returnBtoDeliveryOrderitemsDto = returnBtoDeliveryOrderPostDto.ReturnBtoDeliveryOrderItemsPostDtos;
                //var getBtoNumber = returnBtoDeliveryOrderPostDto.BTONumber;
                //var returnBtoNumberCount = await _repository.GetReturnBtoDeliveryOrderByBtoNo(getBtoNumber);


                var returnBtoDeliveryOrderItemsDtoList = new List<ReturnBtoDeliveryOrderItems>();

                if (returnBtoDeliveryOrderitemsDto != null)
                {
                    Guid guid = Guid.NewGuid();
                    var btohistoryNo = await _bTODeliveryOrderHistoryRepository.GetBTONumberCount(returnBtoDeliveryOrder.BTONumber);

                    for (int i = 0; i < returnBtoDeliveryOrderitemsDto.Count; i++)
                    {

                        ReturnBtoDeliveryOrderItems returnBtoDeliveryOrderItems = _mapper.Map<ReturnBtoDeliveryOrderItems>(returnBtoDeliveryOrderitemsDto[i]);
                        returnBtoDeliveryOrderItems.QtyDistribution = _mapper.Map<List<ReturnDeliveryOrderItemQtyDistribution>>(returnBtoDeliveryOrderitemsDto[i].QtyDistribution);
                        returnBtoDeliveryOrderItems.ReturnQty = returnBtoDeliveryOrderItems.AlreadyReturnQty + returnBtoDeliveryOrderItems.ReturnQty;
                        returnBtoDeliveryOrderItems.AlreadyReturnQty = returnBtoDeliveryOrderItems.AlreadyReturnQty + returnBtoDeliveryOrderItems.ReturnQty;
                        returnBtoDeliveryOrderItems.DispatchQty = returnBtoDeliveryOrderItems.DispatchQty - returnBtoDeliveryOrderItems.ReturnQty;
                        returnBtoDeliveryOrderItemsDtoList.Add(returnBtoDeliveryOrderItems);
                        BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                        if (btohistoryNo != null)
                        {
                            int suffixNumber = int.Parse(btohistoryNo.Substring(btohistoryNo.LastIndexOf("-R") + 2)) + 1;
                            string suffix = "-R" + suffixNumber;
                            returnBtoDeliveryOrderItems.BTONumber += suffix;
                            bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                            returnBtoDeliveryOrder.ReturnBTONumber = returnBtoDeliveryOrderItems.BTONumber;
                        }
                        else
                        {
                            returnBtoDeliveryOrderItems.BTONumber += "-R1";
                            bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                            returnBtoDeliveryOrder.ReturnBTONumber = returnBtoDeliveryOrderItems.BTONumber;
                        }
                        //Update Inventory balanced Quantity

                        //var PartNumber = returnBtoDeliveryOrderitemsDto[i].FGPartNumber;
                        //var BtoNumber = returnBtoDeliveryOrderitemsDto[i].BTONumber;
                        //var getInventoryFGDetailsByItemnumber = await _inventoryRepository.GetInventoryFGDetailsByItemNumber(PartNumber);
                        // decimal ReturnQty = Convert.ToDecimal(returnBtoDeliveryOrderitemsDto[i].ReturnQty);

                        //if (getInventoryFGDetailsByItemnumber != null)
                        //{
                        //    getInventoryFGDetailsByItemnumber.Balance_Quantity = getInventoryFGDetailsByItemnumber.Balance_Quantity + ReturnQty;

                        //    _inventoryRepository.Update(getInventoryFGDetailsByItemnumber);
                        //    _inventoryRepository.SaveAsync();
                        //}
                        //else
                        //{
                        foreach (var eachbin in returnBtoDeliveryOrderItems.QtyDistribution)
                        {
                            var exInv = await _inventoryRepository.GetInventorybyItemProjectWarehouseLocation(returnBtoDeliveryOrderItems.FGPartNumber, eachbin.ProjectNumber, eachbin.Warehouse, eachbin.Location);
                            if (exInv == null)
                            {
                                Inventory inventory = new Inventory();
                                inventory.PartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                                inventory.MftrPartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                                inventory.Description = returnBtoDeliveryOrderItemsDtoList[i].Description;
                                inventory.ProjectNumber = eachbin.ProjectNumber;
                                inventory.Balance_Quantity = eachbin.DistributingQty;
                                inventory.UOM = returnBtoDeliveryOrderItemsDtoList[i].UOM;
                                inventory.IsStockAvailable = true;
                                inventory.Warehouse = eachbin.Warehouse;
                                inventory.Location = eachbin.Location;
                                inventory.GrinNo = returnBtoDeliveryOrderItems.BTONumber;
                                inventory.GrinPartId = 0;
                                inventory.PartType = PartType.FG;
                                inventory.GrinMaterialType = "";
                                inventory.ReferenceID = returnBtoDeliveryOrderItems.BTONumber;
                                inventory.ReferenceIDFrom = "Return BTO Delivery Order";
                                inventory.shopOrderNo = "";
                                //inventory.PartType = returnBtoDeliveryOrderItems.PartType;

                                await _inventoryRepository.CreateInventory(inventory);
                                _inventoryRepository.SaveAsync();
                            }
                            else
                            {
                                exInv.ReferenceID = returnBtoDeliveryOrderItems.BTONumber;
                                exInv.ReferenceIDFrom = "Return BTO Delivery Order";
                                exInv.IsStockAvailable = true;
                                exInv.Balance_Quantity += eachbin.DistributingQty;
                                await _inventoryRepository.UpdateInventory(exInv);
                                _inventoryRepository.SaveAsync();

                            }
                            //}

                            InventoryTranction inventoryTranction = new InventoryTranction();
                            inventoryTranction.PartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                            inventoryTranction.MftrPartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                            inventoryTranction.Description = returnBtoDeliveryOrderItemsDtoList[i].Description;
                            inventoryTranction.Issued_Quantity = eachbin.DistributingQty;
                            inventoryTranction.UOM = returnBtoDeliveryOrderItemsDtoList[i].UOM;
                            inventoryTranction.Issued_DateTime = DateTime.Now;
                            inventoryTranction.ReferenceID = returnBtoDeliveryOrderItems.BTONumber;
                            inventoryTranction.ReferenceIDFrom = "Return BTO Delivery Order";
                            inventoryTranction.Issued_By = _createdBy;
                            inventoryTranction.From_Location = "BTO";
                            inventoryTranction.TO_Location = eachbin.Location;
                            inventoryTranction.Remarks = "Return BTO Delivery Order";
                            inventoryTranction.Warehouse = eachbin.Warehouse;
                            inventoryTranction.PartType = PartType.FG;

                            var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);

                            await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                            _inventoryTranctionRepository.SaveAsync();


                            //update Dispatch Qty in Bto Delivery Order Table
                            int getBtoPartsId = returnBtoDeliveryOrderitemsDto[i].BtoDeliveryOrderPartsId;
                            var getBtoDeliveryOrderDetails = await _bTODeliveryOrderItemsRepository.GetBtoDelieveryOrderItemDetails(getBtoPartsId);
                            getBtoDeliveryOrderDetails.BalanceDoQty -= eachbin.DistributingQty;
                            getBtoDeliveryOrderDetails.OrderBalanceQty += eachbin.DistributingQty;
                            getBtoDeliveryOrderDetails.DispatchQty -= eachbin.DistributingQty;

                            string[] strs1 = getBtoDeliveryOrderDetails.SerialNo.Split(",");
                            string[] strs2 = returnBtoDeliveryOrderitemsDto[i].SerialNo.Split(",");

                            List<string> result = new List<string>();

                            foreach (string elem in strs1)
                            {
                                if (strs2.Contains(elem))
                                {
                                    strs2 = strs2.Where(e => e != elem).ToArray();
                                }
                                else
                                {
                                    result.Add(elem);
                                }
                            }

                            string resultd = string.Join(",", result);
                            //String[] strs1 = getBtoDeliveryOrderDetails.SerialNo.Split(",");
                            //String[] strs2 = returnBtoDeliveryOrderitemsDto[i].SerialNo.Split(",");
                            //var res = strs1.Except(strs2).Union(strs2.Except(strs1));
                            //String resultd = String.Join(",", res);
                            getBtoDeliveryOrderDetails.SerialNo = resultd;

                            // Add return details in to btodeliveryorderhistory table

                            var returnSerialNumber = returnBtoDeliveryOrderitemsDto[i].SerialNo;

                            // BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                            bTODeliveryOrderHistory = new BTODeliveryOrderHistory();

                            // Implement the logic to Add suffix -R for BTO exists in history

                            //if (btohistoryNo != null)
                            //{

                            //    string suffix = "-R1" + (btohistoryNo + 1);
                            //    returnBtoDeliveryOrderItems.BTONumber += suffix;
                            //    bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                            //}
                            //else
                            //{

                            //    returnBtoDeliveryOrderItems.BTONumber += "-R";
                            //    bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                            //}

                            //if (btohistoryNo != null)
                            //{
                            //    int suffixNumber = int.Parse(btohistoryNo.Substring(btohistoryNo.LastIndexOf("-R") + 2)) + 1;
                            //    string suffix = "-R" + suffixNumber;
                            //    returnBtoDeliveryOrderItems.BTONumber += suffix;
                            bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                            //}
                            //else
                            //{
                            //    returnBtoDeliveryOrderItems.BTONumber += "-R1";
                            //    bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                            //}
                            bTODeliveryOrderHistory.CustomerName = returnBtoDeliveryOrder.CustomerName;
                            bTODeliveryOrderHistory.CustomerAliasName = returnBtoDeliveryOrder.CustomerAliasName;
                            bTODeliveryOrderHistory.CustomerId = returnBtoDeliveryOrder.CustomerId;
                            bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrder.BTONumber;
                            bTODeliveryOrderHistory.PONumber = returnBtoDeliveryOrder.PONumber;
                            bTODeliveryOrderHistory.IssuedTo = returnBtoDeliveryOrder.IssuedTo;
                            bTODeliveryOrderHistory.DODate = Convert.ToDateTime(returnBtoDeliveryOrder.CreatedOn);
                            bTODeliveryOrderHistory.FGItemNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                            bTODeliveryOrderHistory.SalesOrderId = getBtoDeliveryOrderDetails.SalesOrderId;
                            bTODeliveryOrderHistory.Description = returnBtoDeliveryOrderItemsDtoList[i].Description;
                            bTODeliveryOrderHistory.BalanceDoQty = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].BalanceQty);
                            bTODeliveryOrderHistory.UnitPrice = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].UnitPrice);
                            bTODeliveryOrderHistory.UOC = returnBtoDeliveryOrderItemsDtoList[i].UOC;
                            bTODeliveryOrderHistory.UOM = returnBtoDeliveryOrderItemsDtoList[i].UOM;
                            bTODeliveryOrderHistory.FGOrderQty = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].FGOrderQty);
                            bTODeliveryOrderHistory.OrderBalanceQty = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].OrderBalanceQty);
                            bTODeliveryOrderHistory.FGStock = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].FGStock);
                            bTODeliveryOrderHistory.Discount = getBtoDeliveryOrderDetails.Discount;
                            bTODeliveryOrderHistory.NetValue = getBtoDeliveryOrderDetails.NetValue;
                            bTODeliveryOrderHistory.Location = eachbin.Location;
                            bTODeliveryOrderHistory.Warehouse = eachbin.Warehouse;
                            bTODeliveryOrderHistory.DispatchQty = eachbin.DistributingQty;
                            //bTODeliveryOrderHistory.InvoicedQty = getBtoDeliveryOrderDetails.InvoicedQty;
                            bTODeliveryOrderHistory.SerialNo = returnSerialNumber;
                            bTODeliveryOrderHistory.Remark = returnBtoDeliveryOrderitemsDto[i].Remarks;
                            bTODeliveryOrderHistory.UniqeId = guid.ToString();

                            var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);

                            await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);
                            _bTODeliveryOrderHistoryRepository.SaveAsync();


                            _bTODeliveryOrderItemsRepository.Update(getBtoDeliveryOrderDetails);
                            _bTODeliveryOrderItemsRepository.SaveAsync();

                        }
                    }
                }

                returnBtoDeliveryOrder.ReturnBtoDeliveryOrderItems = returnBtoDeliveryOrderItemsDtoList;

                await _repository.CreateReturnBtoDeliveryOrder(returnBtoDeliveryOrder);
                _repository.SaveAsync();


                //update balance qty and dispatch qty in sales order table for return bto concept

                var btoDeliveryReturnDetails = _mapper.Map<List<BtoDOReturnQtyDetailsDto>>(returnBtoDeliveryOrderitemsDto);

                var json = JsonConvert.SerializeObject(btoDeliveryReturnDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                // Include the token in the Authorization header
                var tokenValues = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(tokenValues) && tokenValues.StartsWith("Bearer "))
                {
                    var token = tokenValues.Substring(7);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "ReturnDOUpdateDispatchDetails"), data);

                serviceResponse.Data = null;
                serviceResponse.Message = " ReturnBTODeliveryOrder created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateReturnBtoDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateReturnBtoDeliveryOrder_AV([FromBody] ReturnBtoDeliveryOrderPostDto returnBtoDeliveryOrderPostDto)
        {
            ServiceResponse<ReturnBtoDeliveryOrderPostDto> serviceResponse = new ServiceResponse<ReturnBtoDeliveryOrderPostDto>();
            try
            {
                if (returnBtoDeliveryOrderPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReturnBtoDeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ReturnBtoDeliveryOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ReturnBtoDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ReturnBtoDeliveryOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }


                var returnBtoDeliveryOrder = _mapper.Map<ReturnBtoDeliveryOrder>(returnBtoDeliveryOrderPostDto);

                var returnBtoDeliveryOrderitemsDto = returnBtoDeliveryOrderPostDto.ReturnBtoDeliveryOrderItemsPostDtos;
                //var getBtoNumber = returnBtoDeliveryOrderPostDto.BTONumber;
                //var returnBtoNumberCount = await _repository.GetReturnBtoDeliveryOrderByBtoNo(getBtoNumber);


                var returnBtoDeliveryOrderItemsDtoList = new List<ReturnBtoDeliveryOrderItems>();

                if (returnBtoDeliveryOrderitemsDto != null)
                {
                    Guid guid = Guid.NewGuid();
                    var btohistoryNo = await _bTODeliveryOrderHistoryRepository.GetBTONumberCount(returnBtoDeliveryOrder.BTONumber);
                    for (int i = 0; i < returnBtoDeliveryOrderitemsDto.Count; i++)
                    {

                        ReturnBtoDeliveryOrderItems returnBtoDeliveryOrderItems = _mapper.Map<ReturnBtoDeliveryOrderItems>(returnBtoDeliveryOrderitemsDto[i]);
                        returnBtoDeliveryOrderItems.ReturnQty = returnBtoDeliveryOrderItems.AlreadyReturnQty + returnBtoDeliveryOrderItems.ReturnQty;
                        returnBtoDeliveryOrderItems.AlreadyReturnQty = returnBtoDeliveryOrderItems.AlreadyReturnQty + returnBtoDeliveryOrderItems.ReturnQty;
                        returnBtoDeliveryOrderItems.DispatchQty = returnBtoDeliveryOrderItems.DispatchQty - returnBtoDeliveryOrderItems.ReturnQty;
                        returnBtoDeliveryOrderItemsDtoList.Add(returnBtoDeliveryOrderItems);

                        //Update Inventory balanced Quantity

                        var PartNumber = returnBtoDeliveryOrderitemsDto[i].FGPartNumber;
                        var BtoNumber = returnBtoDeliveryOrderitemsDto[i].BTONumber;
                        var getInventoryFGDetailsByItemnumber = await _inventoryRepository.GetInventoryFGDetailsByItemNumber(PartNumber);
                        decimal ReturnQty = Convert.ToDecimal(returnBtoDeliveryOrderitemsDto[i].ReturnQty);

                        if (getInventoryFGDetailsByItemnumber != null)
                        {
                            getInventoryFGDetailsByItemnumber.Balance_Quantity = getInventoryFGDetailsByItemnumber.Balance_Quantity + ReturnQty;

                            _inventoryRepository.Update(getInventoryFGDetailsByItemnumber);
                            _inventoryRepository.SaveAsync();
                        }
                        else
                        {
                            Inventory inventory = new Inventory();
                            inventory.PartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                            inventory.MftrPartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                            inventory.Description = returnBtoDeliveryOrderItemsDtoList[i].Description;
                            inventory.ProjectNumber = "";
                            inventory.Balance_Quantity = ReturnQty;
                            inventory.UOM = returnBtoDeliveryOrderItemsDtoList[i].UOM;
                            inventory.IsStockAvailable = true;
                            inventory.Warehouse = "FG";
                            inventory.Location = "FG";
                            inventory.GrinNo = returnBtoDeliveryOrderItems.BTONumber;
                            inventory.GrinPartId = 0;
                            //inventory.PartType = "";
                            inventory.GrinMaterialType = "";
                            inventory.ReferenceID = returnBtoDeliveryOrderItems.BTONumber;
                            inventory.ReferenceIDFrom = "From BTO Delivery Order";
                            inventory.shopOrderNo = "";
                            //inventory.PartType = returnBtoDeliveryOrderItems.PartType;

                            await _inventoryRepository.CreateInventory(inventory);
                            _inventoryRepository.SaveAsync();
                        }

                        InventoryTranction inventoryTranction = new InventoryTranction();
                        inventoryTranction.PartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                        inventoryTranction.MftrPartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                        inventoryTranction.Description = returnBtoDeliveryOrderItemsDtoList[i].Description;
                        inventoryTranction.Issued_Quantity = ReturnQty;
                        inventoryTranction.UOM = returnBtoDeliveryOrderItemsDtoList[i].UOM;
                        inventoryTranction.Issued_DateTime = DateTime.Now;
                        inventoryTranction.ReferenceID = returnBtoDeliveryOrderItems.BTONumber;
                        inventoryTranction.ReferenceIDFrom = "Return BTO Delivery Order";
                        inventoryTranction.Issued_By = _createdBy;
                        inventoryTranction.From_Location = "BTO";
                        inventoryTranction.TO_Location = "FG";
                        inventoryTranction.Remarks = "Return BTO";
                        inventoryTranction.Warehouse = "FG";
                        //inventoryTranction.PartType = returnBtoDeliveryOrderItems.PartType;

                        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);

                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                        _inventoryTranctionRepository.SaveAsync();


                        //update Dispatch Qty in Bto Delivery Order Table
                        int getBtoPartsId = returnBtoDeliveryOrderitemsDto[i].BtoDeliveryOrderPartsId;
                        var getBtoDeliveryOrderDetails = await _bTODeliveryOrderItemsRepository.GetBtoDelieveryOrderItemDetails(getBtoPartsId);
                        getBtoDeliveryOrderDetails.BalanceDoQty -= ReturnQty;
                        getBtoDeliveryOrderDetails.OrderBalanceQty += ReturnQty;
                        getBtoDeliveryOrderDetails.DispatchQty -= ReturnQty;

                        string[] strs1 = getBtoDeliveryOrderDetails.SerialNo.Split(",");
                        string[] strs2 = returnBtoDeliveryOrderitemsDto[i].SerialNo.Split(",");

                        List<string> result = new List<string>();

                        foreach (string elem in strs1)
                        {
                            if (strs2.Contains(elem))
                            {
                                strs2 = strs2.Where(e => e != elem).ToArray();
                            }
                            else
                            {
                                result.Add(elem);
                            }
                        }

                        string resultd = string.Join(",", result);
                        //String[] strs1 = getBtoDeliveryOrderDetails.SerialNo.Split(",");
                        //String[] strs2 = returnBtoDeliveryOrderitemsDto[i].SerialNo.Split(",");
                        //var res = strs1.Except(strs2).Union(strs2.Except(strs1));
                        //String resultd = String.Join(",", res);
                        getBtoDeliveryOrderDetails.SerialNo = resultd;

                        // Add return details in to btodeliveryorderhistory table

                        var returnSerialNumber = returnBtoDeliveryOrderitemsDto[i].SerialNo;

                        BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();

                        // Implement the logic to Add suffix -R for BTO exists in history

                        //if (btohistoryNo != null)
                        //{

                        //    string suffix = "-R1" + (btohistoryNo + 1);
                        //    returnBtoDeliveryOrderItems.BTONumber += suffix;
                        //    bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                        //}
                        //else
                        //{

                        //    returnBtoDeliveryOrderItems.BTONumber += "-R";
                        //    bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                        //}
                        if (btohistoryNo != null)
                        {
                            int suffixNumber = int.Parse(btohistoryNo.Substring(btohistoryNo.LastIndexOf("-R") + 2)) + 1;
                            string suffix = "-R" + suffixNumber;
                            returnBtoDeliveryOrderItems.BTONumber += suffix;
                            bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                        }
                        else
                        {
                            returnBtoDeliveryOrderItems.BTONumber += "-R1";
                            bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                        }

                        bTODeliveryOrderHistory.CustomerName = returnBtoDeliveryOrder.CustomerName;
                        bTODeliveryOrderHistory.CustomerAliasName = returnBtoDeliveryOrder.CustomerAliasName;
                        bTODeliveryOrderHistory.CustomerId = returnBtoDeliveryOrder.CustomerId;
                        bTODeliveryOrderHistory.PONumber = returnBtoDeliveryOrder.PONumber;
                        bTODeliveryOrderHistory.IssuedTo = returnBtoDeliveryOrder.IssuedTo;
                        bTODeliveryOrderHistory.DODate = Convert.ToDateTime(returnBtoDeliveryOrder.CreatedOn);
                        bTODeliveryOrderHistory.FGItemNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                        bTODeliveryOrderHistory.SalesOrderId = getBtoDeliveryOrderDetails.SalesOrderId;
                        bTODeliveryOrderHistory.Description = returnBtoDeliveryOrderItemsDtoList[i].Description;
                        bTODeliveryOrderHistory.BalanceDoQty = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].BalanceQty);
                        bTODeliveryOrderHistory.UnitPrice = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].UnitPrice);
                        bTODeliveryOrderHistory.UOC = returnBtoDeliveryOrderItemsDtoList[i].UOC;
                        bTODeliveryOrderHistory.UOM = returnBtoDeliveryOrderItemsDtoList[i].UOM;
                        bTODeliveryOrderHistory.FGOrderQty = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].FGOrderQty);
                        bTODeliveryOrderHistory.OrderBalanceQty = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].OrderBalanceQty);
                        bTODeliveryOrderHistory.FGStock = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].FGStock);
                        bTODeliveryOrderHistory.Discount = getBtoDeliveryOrderDetails.Discount;
                        bTODeliveryOrderHistory.NetValue = getBtoDeliveryOrderDetails.NetValue;
                        bTODeliveryOrderHistory.DispatchQty = ReturnQty;
                        bTODeliveryOrderHistory.InvoicedQty = getBtoDeliveryOrderDetails.InvoicedQty;
                        bTODeliveryOrderHistory.SerialNo = returnSerialNumber;
                        bTODeliveryOrderHistory.Remark = returnBtoDeliveryOrderitemsDto[i].Remarks;
                        bTODeliveryOrderHistory.UniqeId = guid.ToString();

                        var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);

                        await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);
                        _bTODeliveryOrderHistoryRepository.SaveAsync();


                        _bTODeliveryOrderItemsRepository.Update(getBtoDeliveryOrderDetails);
                        _bTODeliveryOrderItemsRepository.SaveAsync();

                    }
                }

                returnBtoDeliveryOrder.ReturnBtoDeliveryOrderItems = returnBtoDeliveryOrderItemsDtoList;

                //await _repository.CreateReturnBtoDeliveryOrder(returnBtoDeliveryOrder);
                //_repository.SaveAsync();


                //update balance qty and dispatch qty in sales order table for return bto concept

                var btoDeliveryReturnDetails = _mapper.Map<List<BtoDOReturnQtyDetailsDto>>(returnBtoDeliveryOrderitemsDto);

                var json = JsonConvert.SerializeObject(btoDeliveryReturnDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                // Include the token in the Authorization header
                var tokenValues = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(tokenValues) && tokenValues.StartsWith("Bearer "))
                {
                    var token = tokenValues.Substring(7);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "ReturnDOUpdateDispatchDetails"), data);

                serviceResponse.Data = null;
                serviceResponse.Message = " ReturnBTODeliveryOrder created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateReturnBtoDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReturnBtoDeliveryOrder(int id, [FromBody] ReturnBtoDeliveryOrderUpdateDto returnBtoDeliveryOrderUpdateDto)
        {
            ServiceResponse<ReturnBtoDeliveryOrderUpdateDto> serviceResponse = new ServiceResponse<ReturnBtoDeliveryOrderUpdateDto>();
            try
            {
                if (returnBtoDeliveryOrderUpdateDto is null)
                {
                    _logger.LogError("Update ReturnBtoDeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ReturnBtoDeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update ReturnBtoDeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ReturnBtoDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var returnBtoDeliveryOrderbyId = await _repository.GetReturnBtoDeliveryOrderById(id);
                if (returnBtoDeliveryOrderbyId is null)
                {
                    _logger.LogError($"Update ReturnBtoDeliveryOrder with id: {id}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update ReturnBtoDeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var returnBtoDeliveryOrder = _mapper.Map<ReturnBtoDeliveryOrder>(returnBtoDeliveryOrderbyId);
                var returnBtoDeliveryOrderitemsDto = returnBtoDeliveryOrderUpdateDto.ReturnBtoDeliveryOrderItemsUpdateDtos;
                var returnBtoDeliveryOrderitemsList = new List<ReturnBtoDeliveryOrderItems>();

                if (returnBtoDeliveryOrderitemsDto != null)
                {
                    for (int i = 0; i < returnBtoDeliveryOrderitemsDto.Count; i++)
                    {
                        ReturnBtoDeliveryOrderItems returnDeliveryOrderItems = _mapper.Map<ReturnBtoDeliveryOrderItems>(returnBtoDeliveryOrderitemsDto[i]);
                        returnBtoDeliveryOrderitemsList.Add(returnDeliveryOrderItems);
                    }
                }

                returnBtoDeliveryOrder.ReturnBtoDeliveryOrderItems = returnBtoDeliveryOrderitemsList;
                var updateReturnBtoDeliveryOrder = _mapper.Map(returnBtoDeliveryOrderUpdateDto, returnBtoDeliveryOrder);

                string result = await _repository.UpdateReturnBtoDeliveryOrder(updateReturnBtoDeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " ReturnBTODeliveryOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside updateReturnBtoDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReturnBtoDeliveryOrder(int id)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var deleteReturnBtoDeliveryOrder = await _repository.GetReturnBtoDeliveryOrderById(id);
                if (deleteReturnBtoDeliveryOrder == null)
                {
                    _logger.LogError($"Delete ReturnBtoDeliveryOrder with id: {id}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete ReturnBtoDeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteReturnBtoDeliveryOrder(deleteReturnBtoDeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " ReturnBtoDeliveryOrder Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteReturnBtoDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetReturnBTODeliveryOrderNumberList()
        {
            ServiceResponse<IEnumerable<ReturnBTONumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnBTONumberListDto>>();

            try
            {
                var returnBTODeliveryOrderNumberList = await _repository.GetReturnBtoDeliveryOrderNumberList();
                if (returnBTODeliveryOrderNumberList == null)
                {
                    _logger.LogError("ReturnBtoDeliveryOrderNo Not Found");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReturnBtoDeliveryOrderNo Not Found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo("Returned ReturnBtoDeliveryOrderNos");
                    var result = _mapper.Map<IEnumerable<ReturnBTONumberListDto>>(returnBTODeliveryOrderNumberList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ReturnBtoDeliveryOrderNos Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong GetReturnBtoDeliveryOrderNumberList Details: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }

}
