using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForecastSourcingItemsRepository
    {
        Task<PagedList<ForecastSourcingItems>> GetAllForecastSourcingItems(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<ForecastSourcingItems> GetForecastSourcingItemsById(int id);
        Task<int?> CreateForecastSourcingItems(ForecastSourcingItems forecastSourcingItems);
        Task<string> UpdateForecastSourcingItems(ForecastSourcingItems forecastSourcingItems);
        Task<string> DeleteForecastSourcingItems(ForecastSourcingItems forecastSourcingItems);
    }
}
