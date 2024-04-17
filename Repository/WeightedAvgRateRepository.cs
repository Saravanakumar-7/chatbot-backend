using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class WeightedAvgRateRepository : RepositoryBase<WeightedAvgRate>, IWeightedAvgRateRepository
    {
        private TipsMasterDbContext _tipsMasterServiceDbContext;
        public WeightedAvgRateRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsMasterServiceDbContext = repositoryContext;
        }

        public async Task<IEnumerable<WeightedAvgRate>> GetWeighted_AvgRateDetails(SearchParames searchParams)
        {   
                 var weightedAvgDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => (string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Itemnumber.Contains(searchParams.SearchValue)));

                   return weightedAvgDetails;
        }
        public async Task<WeightedAvgRate> GetWeightedAvgRateDetailsByItemNumber(string ItemNumber)
        {
            var getItemMasterByItemNo = await _tipsMasterServiceDbContext.weighted_avg_rate.
                                        Where(x => x.Itemnumber == ItemNumber)
                                       .FirstOrDefaultAsync();

            return getItemMasterByItemNo;
        }
    }
}

