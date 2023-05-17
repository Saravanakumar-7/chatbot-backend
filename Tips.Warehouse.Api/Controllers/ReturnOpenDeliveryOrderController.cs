using System.Net;
using AutoMapper;
using Contracts;
using Entities;
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
    public class ReturnOpenDeliveryOrderController : ControllerBase
    {
        private IReturnOpenDeliveryOrderRepository _repository;
        private IInventoryRepository _inventoryRepository;
        private IOpenDeliveryOrderHistoryRepository _openDeliveryOrderHistoryRepository;
        private IReturnOpenDeliveryOrderPartsRepository _openDeliveryOrderPartsRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;

        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ReturnOpenDeliveryOrderController(IReturnOpenDeliveryOrderRepository repository, 
            IInventoryTranctionRepository inventoryTranctionRepository, IOpenDeliveryOrderHistoryRepository openDeliveryOrderHistoryRepository,
            IReturnOpenDeliveryOrderPartsRepository openDeliveryOrderPartsRepository, IInventoryRepository inventoryRepository, HttpClient httpClient, 
            IConfiguration config, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _inventoryRepository = inventoryRepository;
            _openDeliveryOrderPartsRepository = openDeliveryOrderPartsRepository;
            _openDeliveryOrderHistoryRepository = openDeliveryOrderHistoryRepository;
            _inventoryTranctionRepository = inventoryTranctionRepository;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenDeliveryOrderHistoryDetails([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderHistory>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderHistory>>();
            try
            {
                var openDeliveryOrderHistoryDetails = await _openDeliveryOrderHistoryRepository.GetAllOpenDeliveryOrderHistoryDetails(pagingParameter);
                var metadata = new
                {
                    openDeliveryOrderHistoryDetails.TotalCount,
                    openDeliveryOrderHistoryDetails.PageSize,
                    openDeliveryOrderHistoryDetails.CurrentPage,
                    openDeliveryOrderHistoryDetails.HasNext,
                    openDeliveryOrderHistoryDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all OpenDeliveryOrderHistoryDetails");
                var result = _mapper.Map<IEnumerable<OpenDeliveryOrderHistory>>(openDeliveryOrderHistoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenDeliveryOrderHistoryDetails";
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
        public async Task<IActionResult> GetAllReturnODODetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<ReturnOpenDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnOpenDeliveryOrderDto>>();
            try
            {
                var returnODODetails = await _repository.GetAllReturnOpenDeliveryOrderDetails(pagingParameter, searchParams);
                var metadata = new
                {
                    returnODODetails.TotalCount,
                    returnODODetails.PageSize,
                    returnODODetails.CurrentPage,
                    returnODODetails.HasNext,
                    returnODODetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ReturnOpenDeliveryOrders");
                var result = _mapper.Map<IEnumerable<ReturnOpenDeliveryOrderDto>>(returnODODetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ReturnOpenDeliveryOrders";
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
        public async Task<IActionResult> GetODOHistoryDetailsById(int id)
        {
            ServiceResponse<OpenDeliveryOrderHistory> serviceResponse = new ServiceResponse<OpenDeliveryOrderHistory>();

            try
            {
                var openDeliveryOrderHistoryDetailById = await _openDeliveryOrderHistoryRepository.GetOpenDeliveryOrderHistoryDetailsById(id);
                if (openDeliveryOrderHistoryDetailById == null)
                {
                    _logger.LogError($"OpenDeliveryOrderHistoryDetailById hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrderHistoryDetailById hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned OpenDeliveryOrderHistoryDetailById : {id}");
                    var result = _mapper.Map<OpenDeliveryOrderHistory>(openDeliveryOrderHistoryDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned OpenDeliveryOrderHistoryDetailById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetODOHistoryDetailsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReturnOpenDeliveryOrderById(int id)
        {
            ServiceResponse<ReturnOpenDeliveryOrderDto> serviceResponse = new ServiceResponse<ReturnOpenDeliveryOrderDto>();
            try
            {
                var returnOpenDeliveryOrderDetailById = await _repository.GetReturnOpenDeliveryOrderById(id);

                if (returnOpenDeliveryOrderDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ReturnOpenDeliveryOrder  hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ReturnOpenDeliveryOrder with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ReturnOpenDeliveryOrder with id: {id}");

                    ReturnOpenDeliveryOrderDto returnOpenDeliveryOrderDto = _mapper.Map<ReturnOpenDeliveryOrderDto>(returnOpenDeliveryOrderDetailById);

                    List<ReturnOpenDeliveryOrderPartsDto> returnOpenDeliveryOrderPartsDtoList = new List<ReturnOpenDeliveryOrderPartsDto>();

                    if (returnOpenDeliveryOrderDetailById.ReturnOpenDeliveryOrderParts != null)
                    {

                        foreach (var openDeliveryOrderitemDetails in returnOpenDeliveryOrderDetailById.ReturnOpenDeliveryOrderParts)
                        {
                            ReturnOpenDeliveryOrderPartsDto returnOpenDeliveryOrderItemsDtos = _mapper.Map<ReturnOpenDeliveryOrderPartsDto>(openDeliveryOrderitemDetails);
                            returnOpenDeliveryOrderPartsDtoList.Add(returnOpenDeliveryOrderItemsDtos);
                        }
                    }

                    returnOpenDeliveryOrderDto.ReturnOpenDeliveryOrderPartsDtos = returnOpenDeliveryOrderPartsDtoList;

                    serviceResponse.Data = returnOpenDeliveryOrderDto;
                    serviceResponse.Message = "Returned ReturnOpenDeliveryOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetReturnOpenDeliveryOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> CreateReturnOpenDeliveryOrder([FromBody] ReturnOpenDeliveryOrderPostDto returnOpenDeliveryOrderPostDto)
        //{
        //    ServiceResponse<ReturnOpenDeliveryOrderPostDto> serviceResponse = new ServiceResponse<ReturnOpenDeliveryOrderPostDto>();
        //    try
        //    {
        //        if (returnOpenDeliveryOrderPostDto is null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "ReturnOpenDeliveryOrder object sent from client is null.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("ReturnOpenDeliveryOrder object sent from client is null.");
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid ReturnOpenDeliveryOrder object sent from client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("Invalid ReturnOpenDeliveryOrder object sent from client.");
        //            return BadRequest(serviceResponse);
        //        }


        //        var returnOpenDeliveryOrder = _mapper.Map<ReturnOpenDeliveryOrder>(returnOpenDeliveryOrderPostDto);

        //        var returnOpenDeliveryOrderitemsDto = returnOpenDeliveryOrderPostDto.ReturnOpenDeliveryOrderPartsPostDtos;

        //        var returnOpenDeliveryOrderPartsDtoList = new List<ReturnOpenDeliveryOrderParts>();

        //        if (returnOpenDeliveryOrderitemsDto != null)
        //        {
        //            Guid guid = Guid.NewGuid();

        //            for (int i = 0; i < returnOpenDeliveryOrderitemsDto.Count; i++)
        //            {

        //                ReturnOpenDeliveryOrderParts returnOpenDeliveryOrderParts = _mapper.Map<ReturnOpenDeliveryOrderParts>(returnOpenDeliveryOrderitemsDto[i]);
        //                returnOpenDeliveryOrderParts.ReturnQty = returnOpenDeliveryOrderParts.AlreadyReturnQty + returnOpenDeliveryOrderParts.ReturnQty;
        //                returnOpenDeliveryOrderParts.AlreadyReturnQty = returnOpenDeliveryOrderParts.AlreadyReturnQty + returnOpenDeliveryOrderParts.ReturnQty;
        //                returnOpenDeliveryOrderParts.DispatchQty = returnOpenDeliveryOrderParts.DispatchQty - returnOpenDeliveryOrderParts.ReturnQty;
        //                returnOpenDeliveryOrderPartsDtoList.Add(returnOpenDeliveryOrderParts);

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReturnOpenDeliveryOrder(int id, [FromBody] ReturnOpenDeliveryOrderUpdateDto returnOpenDeliveryOrderUpdateDto)
        {
            ServiceResponse<ReturnOpenDeliveryOrderUpdateDto> serviceResponse = new ServiceResponse<ReturnOpenDeliveryOrderUpdateDto>();
            try
            {
                if (returnOpenDeliveryOrderUpdateDto is null)
                {
                    _logger.LogError("Update ReturnOpenDeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ReturnOpenDeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update ReturnOpenDeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ReturnOpenDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var returnOpenDeliveryOrderbyId = await _repository.GetReturnOpenDeliveryOrderById(id);
                if (returnOpenDeliveryOrderbyId is null)
                {
                    _logger.LogError($"Update ReturnOpenDeliveryOrder with id: {id}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update ReturnOpenDeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var returnOpenDeliveryOrder = _mapper.Map<ReturnOpenDeliveryOrder>(returnOpenDeliveryOrderbyId);
                var returnOpenDeliveryOrderPartsDto = returnOpenDeliveryOrderUpdateDto.ReturnOpenDeliveryOrderPartsUpdateDtos;
                var returnOpenDeliveryOrderPartsList = new List<ReturnOpenDeliveryOrderParts>();

                if (returnOpenDeliveryOrderPartsDto != null)
                {
                    for (int i = 0; i < returnOpenDeliveryOrderPartsDto.Count; i++)
                    {
                        ReturnOpenDeliveryOrderParts returnDeliveryOrderItems = _mapper.Map<ReturnOpenDeliveryOrderParts>(returnOpenDeliveryOrderPartsDto[i]);
                        returnOpenDeliveryOrderPartsList.Add(returnDeliveryOrderItems);
                    }
                }

                returnOpenDeliveryOrder.ReturnOpenDeliveryOrderParts = returnOpenDeliveryOrderPartsList;
                var updateReturnOpenDeliveryOrder = _mapper.Map(returnOpenDeliveryOrderUpdateDto, returnOpenDeliveryOrder);

                string result = await _repository.UpdateReturnOpenDeliveryOrder(updateReturnOpenDeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " ReturnOpenDeliveryOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside updateReturnOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReturnOpenDeliveryOrder(int id)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();
            try
            {
                var returnOpenDeliveryOrderById = await _repository.GetReturnOpenDeliveryOrderById(id);
                if (returnOpenDeliveryOrderById == null)
                {
                    _logger.LogError($"Delete ReturnOpenDeliveryOrder with id: {id}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete ReturnOpenDeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteReturnOpenDeliveryOrder(returnOpenDeliveryOrderById);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " ReturnOpenDeliveryOrder Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteReturnOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
