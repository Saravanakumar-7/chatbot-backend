using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Net;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Repository;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SalesOrderHistoryController : ControllerBase
    {
        private ISalesOrderMainLevelHistoryRepository _salesOrderMainLevelHistoryRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public SalesOrderHistoryController(ISalesOrderMainLevelHistoryRepository salesOrderMainLevelHistoryRepository , ILoggerManager loggerManager,
                                                     IMapper mapper)
        {
            _salesOrderMainLevelHistoryRepository = salesOrderMainLevelHistoryRepository;
            _logger = loggerManager;
            _mapper = mapper;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateSalesOrderHistory([FromBody] SalesOrder salesOrder)
        //{
        //    ServiceResponse<SalesOrderMainLevelHistory> serviceResponse = new ServiceResponse<SalesOrderMainLevelHistory>();
        //    try
        //    {

        //        if (salesOrder is null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "SalesOrderHistory object sent from client is null.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("SalesOrderHistory object sent from client is null.");
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid SalesOrderHistory object sent from client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("Invalid SalesOrderHistory object sent from client.");
        //            return BadRequest(serviceResponse);
        //        }

        //        var salesOrderMainLevelHistory = _mapper.Map<SalesOrderMainLevelHistory>(salesOrder);
        //        salesOrderMainLevelHistory.SalesOrderId = salesOrder.Id;

        //        var salesOrderItems = salesOrder.SalesOrdersItems;
        //        var SalesOrderItemLevelHistoryList = new List<SalesOrderItemLevelHistory>();
        //        var SalesOrderScheduleDateHistoryList = new List<SalesOrderScheduleDateHistory>();
        //        var SOAdditionalChargesHistoryList = new List<SOAdditionalChargesHistory>();

        //        if (salesOrder.SalesOrderAdditionalCharges != null)
        //        {
        //            foreach(var SalesOrderAdditionalCharges in salesOrder.SalesOrderAdditionalCharges)
        //            {
        //                SOAdditionalChargesHistory soAdditionalChargesHistory = _mapper.Map<SOAdditionalChargesHistory>(SalesOrderAdditionalCharges);
        //                soAdditionalChargesHistory.SOAdditionalChargeId = SalesOrderAdditionalCharges.Id;
        //                SOAdditionalChargesHistoryList.Add(soAdditionalChargesHistory);
        //            }
        //        }

        //        if (salesOrderItems != null)
        //        {
        //            foreach (var salesOrderItem in salesOrderItems)
        //            {
        //                SalesOrderItemLevelHistory salesOrderItemLevelHistory = _mapper.Map<SalesOrderItemLevelHistory>(salesOrderItem);
        //                salesOrderItemLevelHistory.SalesOrderItemId = salesOrderItem.Id;
        //                SalesOrderItemLevelHistoryList.Add(salesOrderItemLevelHistory);

        //                if (salesOrderItem.ScheduleDates != null)
        //                {
        //                    foreach (var ScheduleDate in salesOrderItem.ScheduleDates)
        //                    {
        //                        SalesOrderScheduleDateHistory salesOrderScheduleDateHistory = _mapper.Map<SalesOrderScheduleDateHistory>(ScheduleDate);
        //                        salesOrderScheduleDateHistory.SOScheduleDateId = ScheduleDate.Id;
        //                        SalesOrderScheduleDateHistoryList.Add(salesOrderScheduleDateHistory);
        //                    }
        //                }
        //            }
        //        }

        //            salesOrderMainLevelHistory.SalesOrderItemsHistory = SalesOrderItemLevelHistoryList;
        //            salesOrderMainLevelHistory.SOAdditionalChargesHistory = SOAdditionalChargesHistoryList;

        //            await _salesOrderMainLevelHistoryRepository.CreateSalesOrderMainLevelHistory(salesOrderMainLevelHistory);
        //            _salesOrderMainLevelHistoryRepository.SaveAsync();

        //            serviceResponse.Data = null;
        //            serviceResponse.Message = " SalesOrderHistory Successfully Created";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(serviceResponse);


        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside CreateSalesOrderHistory action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderMainLevelHistoryRevNoListBySOIdAndRevNo(int SalesOrderId, int RevNo)
        {
            ServiceResponse<IEnumerable<SOHistoryRevNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<SOHistoryRevNoListDto>>();

            try
            {
                var soHistoryRevNoDetailBySOIdAndRevNo = await _salesOrderMainLevelHistoryRepository.GetSalesOrderMainLevelHistoryRevNoListBySalesOrderIdAndRevNo(SalesOrderId, RevNo);
                if (soHistoryRevNoDetailBySOIdAndRevNo == null)
                {
                    _logger.LogError($"SalesOrderHistoryDetail with id: {SalesOrderId}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderHistoryDetail with id: {SalesOrderId}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned GetSalesOrderMainLevelHistoryRevNoListBySOIdAndRevNo with id: {SalesOrderId}");
                    serviceResponse.Data = soHistoryRevNoDetailBySOIdAndRevNo;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderMainLevelHistoryRevNoListBySOIdAndRevNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderMainLevelHistoryDetialsBySOHistoryIdAndRevNo(int SalesOrderHistoryId, int RevNo)
        {
            ServiceResponse<SalesOrderMainLevelHistory> serviceResponse = new ServiceResponse<SalesOrderMainLevelHistory>();

            try
            {
                var soHistoryRevNoDetailBySOIdAndRevNo = await _salesOrderMainLevelHistoryRepository.GetSalesOrderMainLevelHistoryBySalesOrderHistoryIdAndRevNo(SalesOrderHistoryId, RevNo);
                if (soHistoryRevNoDetailBySOIdAndRevNo == null)
                {
                    _logger.LogError($"SalesOrderHistoryDetail with id: {SalesOrderHistoryId}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderHistoryDetail with id: {SalesOrderHistoryId}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned GetSalesOrderMainLevelHistoryDetialsBySOHistoryIdAndRevNo with id: {SalesOrderHistoryId}");
                    soHistoryRevNoDetailBySOIdAndRevNo.SOAdditionalChargesHistory.ForEach(x => x.SalesOrderMainLevelHistory = null);
                    soHistoryRevNoDetailBySOIdAndRevNo.SalesOrderItemLevelHistory.ForEach(x =>
                    {
                        x.SalesOrderMainLevelHistory = null;
                        x.SalesOrderScheduleDateHistory.ForEach(z => z.SalesOrderItemLevelHistory = null);
                    });
                    serviceResponse.Data = soHistoryRevNoDetailBySOIdAndRevNo;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderMainLevelHistoryDetialsBySOHistoryIdAndRevNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
