using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualBasic;
using System.Net;
namespace Tips.Master.Api.Controllers
{
    public class FG_Weighted_AvgCostTask : I_FG_Weighted_AvgCostTask
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IRepositoryWrapperForMaster _repository;
        public FG_Weighted_AvgCostTask(ILoggerManager logger, IMapper mapper, IRepositoryWrapperForMaster repository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Calculate_FG_Weighted_AvgCost()
        {           
            try
            {
                TransferCurrent_FG_Weighted_AvgCost_TO_FG_Weighted_AvgCost_History();

                var production_FGs = await _repository.FG_Weighted_AvgCostRepository.GetFGsAndLatestVersion();
                var Weighted_SAs = await _repository.SA_Weighted_AvgCostRepository.GetAllSA_Weighted_AvgCost();
                foreach (var productinFG in production_FGs.Keys)
                {
                    await Weighted_Calculation(productinFG, production_FGs[productinFG], Weighted_SAs);
                }
                _logger.LogInfo("Calculate_FG_Weighted_AvgCost was Sucessfull ");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Occured In Calculate_FG_Weighted_AvgCost: " + ex.Message);
                throw;
            }
        }
        private async Task Weighted_Calculation(string itemNumber, decimal version, List<SA_Weighted_AvgCost> sA_Weighted_AvgCosts)
        {
            decimal ppQtyAndWeight = 0;
            var bomDetails = await _repository.SA_Weighted_AvgCostRepository.GetEnggBomByItemNoAndRevNo(itemNumber, version);

            if (bomDetails?.EnggChildItems != null)
            {
                foreach (var bomItem in bomDetails.EnggChildItems)
                {
                    if (bomItem.PartType == PartType.PurchasePart)
                    {
                        var ppWeight = await _repository.FG_Weighted_AvgCostRepository.GetPPWeightedAvgCost(bomItem.ItemNumber);
                        if (ppWeight != null) ppQtyAndWeight += (ppWeight.Avg_cost * bomItem.Quantity);
                    }
                    else
                    {
                        var persentSA = sA_Weighted_AvgCosts.Where(x => x.Itemnumber == bomItem.ItemNumber).FirstOrDefault();
                        if (persentSA != null) ppQtyAndWeight += (persentSA.Avg_cost * bomItem.Quantity);
                    }
                }

                var fgWeightedAvgCost = new FG_Weighted_AvgCost
                {
                    Itemnumber = itemNumber,
                    Version = version,
                    Avg_cost = ppQtyAndWeight,
                    update_date_time = DateTime.Now
                };

                await _repository.FG_Weighted_AvgCostRepository.CreateFG_Weighted_AvgCost(fgWeightedAvgCost);
                _repository.Save();
            }
        }
        private async void TransferCurrent_FG_Weighted_AvgCost_TO_FG_Weighted_AvgCost_History()
        {
            var FG_Weighted = await _repository.FG_Weighted_AvgCostRepository.GetAllFG_Weighted_AvgCost();
            var ToFGHistory = _mapper.Map<List<FG_Weighted_AvgCost_History>>(FG_Weighted);
            await _repository.FG_Weighted_AvgCost_History_Repository.TranferToFGWeightedHistory(ToFGHistory);
            await _repository.FG_Weighted_AvgCostRepository.DeleteExistingData();
            _repository.Save();
        }

        //[HttpPost]
        //public async Task<IActionResult> FG_Weighted_AvgCost_Report_withParameter([FromBody] FG_Weighted_AvgCost_ReportDto fg_Weighted_AvgCost_ReportDto)
        //{
        //    ServiceResponse<List<FG_Weighted_AvgCost_Report>> serviceResponse = new ServiceResponse<List<FG_Weighted_AvgCost_Report>>();
        //    try
        //    {
        //        var data = await _repository.FG_Weighted_AvgCostRepository.FG_Weighted_AvgCost_Report_withParameter(fg_Weighted_AvgCost_ReportDto.FGItemNumber);

        //        if (data == null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"FG_Weighted_AvgCost_Report_withParameter hasn't been found.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            _logger.LogError($"FG_Weighted_AvgCost_Report_withParameter hasn't been found in db.");
        //            return NotFound(serviceResponse);
        //        }
        //        else
        //        {
        //            serviceResponse.Data = data;
        //            serviceResponse.Message = "Returned FG_Weighted_AvgCost_Report_withParameter Details";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(serviceResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside FG_Weighted_AvgCost_Report_withParameter action";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> FG_Weighted_AvgCost_Report_withDate(string FromDate, string ToDate)
        //{
        //    ServiceResponse<List<FG_Weighted_AvgCost_Report_withDate>> serviceResponse = new ServiceResponse<List<FG_Weighted_AvgCost_Report_withDate>>();
        //    try
        //    {
        //        var data = await _repository.FG_Weighted_AvgCostRepository.FG_Weighted_AvgCost_Report_withDate(FromDate, ToDate);

        //        if (data == null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"FG_Weighted_AvgCost_Report_withDate hasn't been found.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            _logger.LogError($"FG_Weighted_AvgCost_Report_withDate hasn't been found in db.");
        //            return NotFound(serviceResponse);
        //        }
        //        else
        //        {

        //            serviceResponse.Data = data;
        //            serviceResponse.Message = "Returned FG_Weighted_AvgCost_Report_withDate Details";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(serviceResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside FG_Weighted_AvgCost_Report_withDate action";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
        //[HttpPost]
        //public async Task<IActionResult> Weighted_AvgCost_Report_withParameter([FromBody] FG_Weighted_AvgCost_ReportDto fg_Weighted_AvgCost_ReportDto)
        //{
        //    ServiceResponse<List<Weighted_AvgCost_Report>> serviceResponse = new ServiceResponse<List<Weighted_AvgCost_Report>>();
        //    try
        //    {
        //        var data = await _repository.FG_Weighted_AvgCostRepository.Weighted_AvgCost_Report_withParameter(fg_Weighted_AvgCost_ReportDto.FGItemNumber);

        //        if (data == null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"Weighted_AvgCost_Report_withParameter hasn't been found.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            _logger.LogError($"Weighted_AvgCost_Report_withParameter hasn't been found in db.");
        //            return NotFound(serviceResponse);
        //        }
        //        else
        //        {
        //            serviceResponse.Data = data;
        //            serviceResponse.Message = "Returned Weighted_AvgCost_Report_withParameter Details";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(serviceResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside Weighted_AvgCost_Report_withParameter action";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
    }
}
