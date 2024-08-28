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
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FG_Weighted_AvgCostController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IRepositoryWrapperForMaster _repository;
        public FG_Weighted_AvgCostController(ILoggerManager logger, IMapper mapper, IRepositoryWrapperForMaster repository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Calculate_FG_Weighted_AvgCost()
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                TransferCurrent_FG_Weighted_AvgCost_TO_FG_Weighted_AvgCost_History();

                var production_FGs = await _repository.ReleaseProductBomRepository.GetFGsAndLatestVersion();
                var Weighted_SAs = await _repository.SA_Weighted_AvgCostRepository.GetAllSA_Weighted_AvgCost();
                foreach (var productinFG in production_FGs.Keys)
                {
                    await Weighted_Calculation(productinFG, production_FGs[productinFG], Weighted_SAs);
                }

                serviceResponse.Message = "Calculation of FG_Weighted_AvgCost Successful";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error calculating FG weighted average cost");
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        private async Task Weighted_Calculation(string itemNumber, decimal version, List<SA_Weighted_AvgCost> sA_Weighted_AvgCosts)
        {
            decimal ppQtyAndWeight = 0;
            var bomDetails = await _repository.EnggBomRepository.GetEnggBomByItemNoAndRevNo(itemNumber, version);

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
        [HttpPost]
        private async void TransferCurrent_FG_Weighted_AvgCost_TO_FG_Weighted_AvgCost_History()
        {
            var FG_Weighted = await _repository.FG_Weighted_AvgCostRepository.GetAllFG_Weighted_AvgCost();
            var ToFGHistory = _mapper.Map<List<FG_Weighted_AvgCost_History>>(FG_Weighted);
            await _repository.FG_Weighted_AvgCost_History_Repository.TranferToFGWeightedHistory(ToFGHistory);
            await _repository.FG_Weighted_AvgCostRepository.DeleteExistingData();
            _repository.Save();
        }


    }
}
