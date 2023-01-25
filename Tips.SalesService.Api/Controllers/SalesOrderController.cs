using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Repository;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        private ISalesOrderRepository _repository;
        private ISalesOrderItemsRepository _salesOrderItemsRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public SalesOrderController(ISalesOrderRepository repository, ISalesOrderItemsRepository salesOrderItemsRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _salesOrderItemsRepository = salesOrderItemsRepository;
        }

        // GET: api/<SalesOrderController>
        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrder([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<SalesOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderDto>>();
            try
            {
                var getAllSalesOrder = await _repository.GetAllSalesOrder(pagingParameter);
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                    var salesOrder = await _repository.GetSalesOrderById(id);

                    if (salesOrder == null)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"SalesOrder  hasn't been found in db.";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        _logger.LogError($"SalesOrder with id: {id}, hasn't been found.");
                        return NotFound(serviceResponse);
                    }
                    else
                    {
                        _logger.LogInfo($"Returned owner with id: {id}");
                        SalesOrderDto salesOrderDto = _mapper.Map<SalesOrderDto>(salesOrder);

                        List<SalesOrderItemsDto> salesOrderItemsDtoList = new List<SalesOrderItemsDto>();

                        foreach (var salesOrderItemDetails in salesOrder.SalesOrdersItems)
                        {
                            SalesOrderItemsDto salesOrderItemsDtos = _mapper.Map<SalesOrderItemsDto>(salesOrderItemDetails);
                            salesOrderItemsDtoList.Add(salesOrderItemsDtos);
                        }

                        salesOrderDto.SalesOrderItemsDtos = salesOrderItemsDtoList;
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
        public async Task<IActionResult> CreateSalesOrder([FromBody] SalesOrderPostDto salesOrderPostDto)
        {
            ServiceResponse<SalesOrderPostDto> serviceResponse = new ServiceResponse<SalesOrderPostDto>();
            try
            {
                if (salesOrderPostDto is null)
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
                var createSalesOrder = _mapper.Map<SalesOrder>(salesOrderPostDto);
                var salesOrderItemsDto = salesOrderPostDto.SalesOrderItemsPostDtos;
                var salesOrderItemsList = new List<SalesOrderItems>();
                if (salesOrderItemsDto != null) 
                {
                    for (int i = 0; i < salesOrderItemsDto.Count; i++)
                    {
                        SalesOrderItems salesOrderItems = _mapper.Map<SalesOrderItems>(salesOrderItemsDto[i]);
                        salesOrderItemsList.Add(salesOrderItems);
                    }
                }
                createSalesOrder.SalesOrdersItems = salesOrderItemsList;
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                var newcount = await _repository.GetSONumberAutoIncrementCount(date);

                if (newcount > 0)
                {
                    var number = newcount + 1;
                    string e = String.Format("{0:D4}", number);
                    createSalesOrder.SalesOrderNumber = days + months + years + "SO" + (e);
                }
                else
                {
                    var count = 1;
                    var e = count.ToString("D4");
                    createSalesOrder.SalesOrderNumber = days + months + years + "SO" + (e);
                }
                await _repository.CreateSalesOrder(createSalesOrder);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " SalesOrder Created Successfully";
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

        // PUT api/<PurchaseOrderController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalesOrder(int id, [FromBody] SalesOrderUpdateDto salesOrderUpdateDto)
        {
            ServiceResponse<SalesOrderUpdateDto> serviceResponse = new ServiceResponse<SalesOrderUpdateDto>();
            try
            {
                if (salesOrderUpdateDto is null)
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
                var getSalesOrders = await _repository.GetSalesOrderById(id);
                if (getSalesOrders is null)
                {
                    _logger.LogError($"Update SalesOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update SalesOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var updateSalesOrders = _mapper.Map<SalesOrder>(salesOrderUpdateDto);
                var salesOrderItemsDto = salesOrderUpdateDto.SalesOrderItemsUpdateDtos;
                var salesOrderItemsList = new List<SalesOrderItems>();
                if (salesOrderItemsDto != null) 
                {
                    for (int i = 0; i < salesOrderItemsDto.Count; i++)
                    {
                        SalesOrderItems salesOrderItemsDetail = _mapper.Map<SalesOrderItems>(salesOrderItemsDto[i]);
                        salesOrderItemsList.Add(salesOrderItemsDetail);
                    }
                }
              
                var updateData = _mapper.Map(salesOrderUpdateDto, getSalesOrders);
                updateData.SalesOrdersItems = salesOrderItemsList;    
                string result = await _repository.UpdateSalesOrder(updateData);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " SalesOrder Updated Successfully";
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
                serviceResponse.Message = " SalesOrder Deleted Successfully";
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
                if (getProjectByItemNo == null)
                {
                    _logger.LogError($"ProjectNo with id: {itemNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ProjectNo with id: {itemNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ProjectNumber with id: {itemNo}");
                    var result = _mapper.Map<IEnumerable<ListOfProjectNoDto>>(getProjectByItemNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ProjectNumber Returned Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetProjectNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //get salesorder Detais by CustomerId

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderDetailsByCustomerId(int Customerid)
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
                    var result =  _mapper.Map<IEnumerable<ListofSalesOrderDetails>>(getSalesDetailByCustomerId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned SalesOrder Details Successfully";
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
        //getprojectnumberbyitemnumber

        //getsalesorderDetailByprojectNoanditemNo --
        [HttpGet]
        public async Task<IActionResult> getSalesOrderDetailByProjectNoandItemNo(string ItemNo, string ProjectNo)
         {
            ServiceResponse<IEnumerable<GetSalesOrderDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<GetSalesOrderDetailsDto>>();

            try
            {
                var getSalesDetail = await _salesOrderItemsRepository.getSalesOrderDetailByProjectNoandItemNo(ItemNo, ProjectNo);
                if (getSalesDetail == null)
                {
                    _logger.LogError($"SalesOrderDetail with id: {ItemNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderDetail with id: {ItemNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SalesOrderDetail with id: {ItemNo}");
                    var result = _mapper.Map<IEnumerable<GetSalesOrderDetailsDto>>(getSalesDetail);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned SalesOrder Details Successfully";
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
        //getsalesorderdetailbyitemnoandsalesorderId


        //[HttpGet]
        //public async Task<IActionResult> getSalesOrderDetailBySalesOrderIdNoandItemNo(string ItemNo, string SalesOrderId)
        //{
        //    ServiceResponse<GetSalesOrderGSTListDto> serviceResponse = new ServiceResponse<GetSalesOrderGSTListDto>();

        //    try
        //    {
        //        var getSalesDetail = await _salesOrderItemsRepository.getSOBySalesOrderIdNoandItemNo(ItemNo, SalesOrderId);
        //        if (getSalesDetail == null)
        //        {
        //            _logger.LogError($"SalesOrderDetail with id: {ItemNo}, hasn't been found in db.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"SalesOrderDetail with id: {ItemNo}, hasn't been found in db.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound();
        //        }
        //        else
        //        {
        //            _logger.LogInfo($"Returned SalesOrderDetail with id: {ItemNo}");
        //            var result = _mapper.Map<GetSalesOrderDetailsDto>(getSalesDetail);
        //            serviceResponse.Data = result;
        //            serviceResponse.Message = "Success";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside SalesDetail action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Inter server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, "Internal server error");
        //    }
        //}


        //public Task<IActionResult> UpdateSOBasedOnCreatingDO()
        //{
        //    return null;
        //}

        //public Task<IActionResult> UpdateSOBasedOnCreatingShopOrder()
        //{
        //    return null;

        //}




    }


}

