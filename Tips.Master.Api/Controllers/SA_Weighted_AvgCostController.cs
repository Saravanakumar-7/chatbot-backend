using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SA_Weighted_AvgCostController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IRepositoryWrapperForMaster _repository;
        public SA_Weighted_AvgCostController(ILoggerManager logger, IMapper mapper, IRepositoryWrapperForMaster repository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> Calculate_SA_Weighted_AvgCost()
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                TransferCurrent_SA_Weighted_AvgCost_TO_SA_Weighted_AvgCost_History();
                var production_SAs = await _repository.ReleaseProductBomRepository.GetSAsAndLatestVersion();
                foreach (var productinSA in production_SAs.Keys)
                {
                   var SAValue= Weighted_Calculation(productinSA, production_SAs[productinSA],production_SAs);
                }
                serviceResponse.Data = null;
                serviceResponse.Message = "Calculation of SA_Weighted_AvgCost Successfull";
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
        private async void TransferCurrent_SA_Weighted_AvgCost_TO_SA_Weighted_AvgCost_History()
        {
            var SA_Weighted = await _repository.SA_Weighted_AvgCostRepository.GetAllSA_Weighted_AvgCost();
            var ToSAHistory = _mapper.Map<List<SA_Weighted_AvgCost_History>>(SA_Weighted);
            await _repository.SA_Weighted_AvgCost_History_Repository.TranferToSAWeightedHistory(ToSAHistory);
            await _repository.SA_Weighted_AvgCostRepository.DeleteExistingData();
            _repository.SaveAsync();
        }
        private async Task<decimal> Weighted_Calculation(string ItemNumber, decimal Version, Dictionary<string,decimal> ProductionBOMList)
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
                        if(PPWeight!=null) ppQtyandWeight += (PPWeight.Avg_cost * bomitems.Quantity);
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
                    update_date_time=DateTime.Now
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
    }
}
