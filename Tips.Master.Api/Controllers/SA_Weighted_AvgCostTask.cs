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
    public class SA_Weighted_AvgCostTask : I_SA_Weighted_AvgCostTask
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IRepositoryWrapperForMaster _repository;
        public SA_Weighted_AvgCostTask(ILoggerManager logger, IMapper mapper, IRepositoryWrapperForMaster repository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Calculate_SA_Weighted_AvgCost()
        {
            try
            {
                TransferCurrent_SA_Weighted_AvgCost_TO_SA_Weighted_AvgCost_History();
                var production_SAs = await _repository.SA_Weighted_AvgCostRepository.GetSAsAndLatestVersion();
                var AllPPWeight = await _repository.FG_Weighted_AvgCostRepository.GetAllPPWeightedAvgCost();
                foreach (var productinSA in production_SAs.Keys)
                {
                    var SAValue = await Weighted_Calculation(productinSA, production_SAs[productinSA], production_SAs, AllPPWeight);
                }
                _logger.LogInfo("Calculate_SA_Weighted_AvgCost was Sucessfull ");

            }
            catch (Exception ex)
            {
                _logger.LogError("Error Occured In Calculate_SA_Weighted_AvgCost: " + ex.Message);
                throw;
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
        private async Task<decimal> Weighted_Calculation(string ItemNumber, decimal Version, Dictionary<string, decimal> ProductionBOMList, List<WeightedAvgRate> AllPPWeight)
        {
            var ExistingSAWeight = await _repository.SA_Weighted_AvgCostRepository.GetSA_Weighted_AvgCost(ItemNumber);
            decimal ppQtyandWeight = 0;
            if (ExistingSAWeight == null)
            {
                var getBomDetais = await _repository.SA_Weighted_AvgCostRepository.GetEnggBomByItemNoAndRevNo(ItemNumber, Version);
                foreach (var bomitems in getBomDetais.EnggChildItems)
                {
                    if (bomitems.PartType == PartType.PurchasePart)
                    {
                        var PPWeight = AllPPWeight.Where(a => a.Itemnumber == bomitems.ItemNumber).FirstOrDefault();//await _repository.SA_Weighted_AvgCostRepository.GetPPWeightedAvgCost(bomitems.ItemNumber);
                        if (PPWeight != null) ppQtyandWeight += (PPWeight.Avg_cost * bomitems.Quantity);
                    }
                    else
                    {
                        if (ProductionBOMList.ContainsKey(bomitems.ItemNumber))
                        {
                            decimal saValue = await Weighted_Calculation(bomitems.ItemNumber, ProductionBOMList[bomitems.ItemNumber], ProductionBOMList);
                            ppQtyandWeight += (saValue * bomitems.Quantity);
                        }
                    }

                }

                SA_Weighted_AvgCost sA_Weighted_AvgCost = new SA_Weighted_AvgCost()
                {
                    Itemnumber = ItemNumber,
                    Version = Version,
                    Avg_cost = ppQtyandWeight,
                    update_date_time = DateTime.Now
                };

                _repository.SA_Weighted_AvgCostRepository.CreateSA_Weighted_AvgCost(sA_Weighted_AvgCost);
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
