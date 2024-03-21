using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class WeightedAvgRateRepository : RepositoryBase<WeightedAvgRate>, IWeightedAvgRateRepository
    {
        public WeightedAvgRateRepository(TipsGrinDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<WeightedAvgRate>> GetWeighted_AvgRateDetails([FromQuery] SearchParams searchParams)
        {   
                 var weightedAvgDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => (string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Itemnumber.Contains(searchParams.SearchValue)));

                   return weightedAvgDetails;
        }
    }
}

