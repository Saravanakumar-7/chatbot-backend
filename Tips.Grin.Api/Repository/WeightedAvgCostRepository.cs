using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
   
    public class WeightedAvgCostRepository : RepositoryBase<WeightedAvgCost>, IWeightedAvgCostRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        public WeightedAvgCostRepository(TipsGrinDbContext repositoryContext, TipsGrinDbContext tipsGrinDbContext) : base(repositoryContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
        }

        public async Task<WeightedAvgCost> CreateWeightedAvgCost(WeightedAvgCost weightedAvgCost)
        {
            var result = await Create(weightedAvgCost);
            return result;
        }

        public async Task<string> UpdateWeightedAvgCostQty(WeightedAvgCost weightedAvgCost)
        {
            var existingEntry = await _tipsGrinDbContext.WeightedAvgCosts.FirstOrDefaultAsync(w => w.ItemNumber == weightedAvgCost.ItemNumber);

            if (existingEntry != null)
            {
                existingEntry.AverageCost = weightedAvgCost.AverageCost;
                _tipsGrinDbContext.WeightedAvgCosts.Update(existingEntry);
                await _tipsGrinDbContext.SaveChangesAsync();

                return $"WeightedAvgCost details for item number {weightedAvgCost.ItemNumber} is updated successfully!";
            }
            else
            {
                return $"No WeightedAvgCost entry found for item number {weightedAvgCost.ItemNumber}!";
            }
        }
        public async Task<string> UpdateWeightedAvgCost(WeightedAvgCost weightedAvgCost)
        {
            Update(weightedAvgCost);
            string result = $"WeightedAvgCost Detail {weightedAvgCost.Id} is updated successfully!";
            return result;
        }
        public async Task<WeightedAvgCost> GetWeightedAvgCostDetailsByItemNo(string itemNo)
        {
            var binningDetailsByGrinNo = await FindByCondition(x => x.ItemNumber == itemNo).FirstOrDefaultAsync();

            return binningDetailsByGrinNo;
        }
    }
}
