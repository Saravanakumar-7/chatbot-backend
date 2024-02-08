using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Contracts
{
    public interface IWeightedAvgRateRepository : IRepositoryBase<WeightedAvgRate>
    {
        Task<PagedList<WeightedAvgRate>> GetWeighted_AvgRateDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams);


    }
}
