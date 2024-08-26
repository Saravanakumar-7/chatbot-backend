using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

                foreach (var productinFG in production_FGs.Keys)
                {
                    var FGValue = await Weighted_Calculation(productinFG, production_FGs[productinFG], production_FGs);
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
 
        private async Task<decimal> Weighted_Calculation(string itemNumber, decimal version, Dictionary<string, decimal> productionBOMList)
        {
            var existingFGWeight = await _repository.FG_Weighted_AvgCostRepository.GetFG_Weighted_AvgCost(itemNumber);
            decimal ppQtyAndWeight = 0;

            if (existingFGWeight == null)
            {
                var bomDetails = await _repository.EnggBomRepository.GetEnggBomByItemNoAndRevNo(itemNumber, version);

                if (bomDetails?.EnggChildItems != null)
                {
                    foreach (var bomItem in bomDetails.EnggChildItems)
                    {
                        if (bomItem.PartType == PartType.PurchasePart)
                        {
                            var ppWeight = await _repository.FG_Weighted_AvgCostRepository.GetPPWeightedAvgCost(bomItem.ItemNumber);
                            if (ppWeight != null)
                            {
                                ppQtyAndWeight += (ppWeight.Avg_cost * bomItem.Quantity);
                            }
                        }
                        else
                        {
                            var saValue = await SAWeighted_Calculation(bomItem.ItemNumber, productionBOMList[bomItem.ItemNumber], productionBOMList);
                            ppQtyAndWeight += (saValue * bomItem.Quantity);
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
                     _repository.SaveAsync();
                }
            }
            else
            {
                ppQtyAndWeight += existingFGWeight.Avg_cost;
            }

            return ppQtyAndWeight;
        }
        private async Task<decimal> SAWeighted_Calculation(string ItemNumber, decimal Version, Dictionary<string, decimal> ProductionBOMList)
        {
            var ExistingSAWeight = await _repository.SA_Weighted_AvgCostRepository.GetSA_Weighted_AvgCost(ItemNumber);
            decimal ppQtyandWeight = 0;
            if (ExistingSAWeight == null)
            {
                var getBomDetais = await _repository.EnggBomRepository.GetEnggBomByItemNoAndRevNo(ItemNumber, Version);
                foreach (var bomitems in getBomDetais.EnggChildItems)
                {
                    if (bomitems.PartType == PartType.PurchasePart)
                    {
                        var PPWeight = await _repository.SA_Weighted_AvgCostRepository.GetPPWeightedAvgCost(bomitems.ItemNumber);
                        if (PPWeight != null) ppQtyandWeight += (PPWeight.Avg_cost * bomitems.Quantity);
                    }
                    else
                    {
                        decimal saValue = await Weighted_Calculation(bomitems.ItemNumber, ProductionBOMList[bomitems.ItemNumber], ProductionBOMList);
                        ppQtyandWeight += (saValue * bomitems.Quantity);
                    }

                }

                SA_Weighted_AvgCost sA_Weighted_AvgCost = new SA_Weighted_AvgCost()
                {
                    Itemnumber = ItemNumber,
                    Version = Version,
                    Avg_cost = ppQtyandWeight,
                    update_date_time = DateTime.Now
                };

                await _repository.SA_Weighted_AvgCostRepository.CreateSA_Weighted_AvgCost(sA_Weighted_AvgCost);
                _repository.SaveAsync();
            }
            else
            {
                ppQtyandWeight += ExistingSAWeight.Avg_cost;
            }
            return ppQtyandWeight;
        }
        private async void TransferCurrent_FG_Weighted_AvgCost_TO_FG_Weighted_AvgCost_History()
        {
            var FG_Weighted = await _repository.FG_Weighted_AvgCostRepository.GetAllFG_Weighted_AvgCost();
            var ToFGHistory = _mapper.Map<List<FG_Weighted_AvgCost_History>>(FG_Weighted);
            await _repository.FG_Weighted_AvgCost_History_Repository.TranferToFGWeightedHistory(ToFGHistory);
            await _repository.FG_Weighted_AvgCostRepository.DeleteExistingData();
            _repository.SaveAsync();
        }

    }
}
