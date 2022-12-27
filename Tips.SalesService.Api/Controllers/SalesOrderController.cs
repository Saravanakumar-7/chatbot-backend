using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        private ISalesOrderRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public SalesOrderController(ISalesOrderRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<SalesOrderController>
        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrder([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<SalesOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderDto>>();
            try
            {
                var listOfSalesOrder = await _repository.GetAllSalesOrder(pagingParameter);
                var metadata = new
                {
                    listOfSalesOrder.TotalCount,
                    listOfSalesOrder.PageSize,
                    listOfSalesOrder.CurrentPage,
                    listOfSalesOrder.HasNext,
                    listOfSalesOrder.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all SalesOrder");
                var result = _mapper.Map<IEnumerable<SalesOrderDto>>(listOfSalesOrder);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrder";
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
            // GET api/<PurchaseOrderController>/5
            [HttpGet("{id}")]
            public async Task<IActionResult> GetSalesOrderById(int id)
            {
                ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();
                try
                {
                    var salesOrderDetails = await _repository.GetSalesOrderById(id);

                    if (salesOrderDetails == null)
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
                        SalesOrderDto salesOrderDto = _mapper.Map<SalesOrderDto>(salesOrderDetails);

                        List<SalesOrderItemsDto> salesOrderItemsDtoList = new List<SalesOrderItemsDto>();
                        foreach (var itemDetails in salesOrderDetails.salesOrdersItems)
                        {
                            SalesOrderItemsDto salesOrderItemsDtos = _mapper.Map<SalesOrderItemsDto>(itemDetails);
                            salesOrderItemsDtoList.Add(salesOrderItemsDtos);
                        }

                        salesOrderDto.salesOrdersItemsDto = salesOrderItemsDtoList;
                        serviceResponse.Data = salesOrderDto;
                        serviceResponse.Message = "Returned SalesOrder";
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
        public async Task<IActionResult> CreateSalesOrder([FromBody] SalesOrderDtoPost salesOrderDtoPost)
        {
            ServiceResponse<SalesOrderDtoPost> serviceResponse = new ServiceResponse<SalesOrderDtoPost>();
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
                var salesOrder = _mapper.Map<SalesOrder>(salesOrderDtoPost);
                var itemsDto = salesOrderDtoPost.salesOrdersItemsDtoPost;
                var itemsDtoList = new List<SalesOrderItems>();
                for (int i = 0; i < itemsDto.Count; i++)
                {
                    SalesOrderItems salesOrderItemsDetails = _mapper.Map<SalesOrderItems>(itemsDto[i]); 
                    itemsDtoList.Add(salesOrderItemsDetails);
                }
                salesOrder.salesOrdersItems = itemsDtoList;
                await _repository.CreateSalesOrder(salesOrder);
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
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<PurchaseOrderController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalesOrder(int id, [FromBody] SalesOrderDtoUpdate salesOrderDtoUpdate)
        {
            ServiceResponse<SalesOrderDtoUpdate> serviceResponse = new ServiceResponse<SalesOrderDtoUpdate>();
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
                var updateSalesOrder = await _repository.GetSalesOrderById(id);
                if (updateSalesOrder is null)
                {
                    _logger.LogError($"Update SalesOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update SalesOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var salesOrderList = _mapper.Map<SalesOrder>(updateSalesOrder);
                var itemsDto = salesOrderDtoUpdate.salesOrdersItemsDtoUpdate;
                var itemsList = new List<SalesOrderItems>();
                for (int i = 0; i < itemsDto.Count; i++)
                {
                    SalesOrderItems poItemsDetails = _mapper.Map<SalesOrderItems>(itemsDto[i]);                  
                    itemsList.Add(poItemsDetails);
                }
                salesOrderList.salesOrdersItems = itemsList;
                var data = _mapper.Map(salesOrderDtoUpdate, salesOrderList);
                string result = await _repository.UpdateSalesOrder(data);
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
                serviceResponse.Message = $"Something went wrong ,try again";
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
                var deleteSalesOrder = await _repository.GetSalesOrderById(id);
                if (deleteSalesOrder == null)
                {
                    _logger.LogError($"Delete SalesOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete SalesOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteSalesOrder(deleteSalesOrder);
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
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
    }

