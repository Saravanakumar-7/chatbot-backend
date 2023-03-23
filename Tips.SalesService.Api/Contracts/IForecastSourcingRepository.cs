using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForecastSourcingRepository
    {
        Task<PagedList<ForecastSourcing>> GetAllForeCastSourcing(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<ForecastSourcing> GetForeCastSourcingById(int id);
        Task<int?> CreateForeCastSourcing(ForecastSourcing forecastSourcing);
        Task<string> UpdateForeCastSourcing(ForecastSourcing forecastSourcing);
        Task<string> DeleteForeCastSourcing(ForecastSourcing forecastSourcing);
        public void SaveAsync();
    }
}
