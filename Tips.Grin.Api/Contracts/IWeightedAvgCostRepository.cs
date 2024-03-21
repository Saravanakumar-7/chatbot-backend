using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IWeightedAvgCostRepository : IRepositoryBase<WeightedAvgCost>
    {
        Task<WeightedAvgCost> CreateWeightedAvgCost(WeightedAvgCost weightedAvgCost);
        Task<string> UpdateWeightedAvgCostQty(WeightedAvgCost weightedAvgCost);
        Task<WeightedAvgCost> GetWeightedAvgCostDetailsByItemNo(string itemNo);
        Task<string> UpdateWeightedAvgCost(WeightedAvgCost weightedAvgCost);
    }
}
