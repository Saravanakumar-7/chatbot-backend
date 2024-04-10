using Entities.Helper;
using Entities;
using Contracts;

namespace Contracts
{
    public interface IWeightedAvgRateRepository : IRepositoryBase<WeightedAvgRate>
    {
        Task<IEnumerable<WeightedAvgRate>> GetWeighted_AvgRateDetails( SearchParames searchParams);
        Task<WeightedAvgRate> GetWeightedAvgRateDetailsByItemNumber(string ItemNumber);


    }
}
