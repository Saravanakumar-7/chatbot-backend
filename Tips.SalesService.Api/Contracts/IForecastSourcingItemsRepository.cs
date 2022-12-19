using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForecastSourcingItemsRepository
    {
        Task<IEnumerable<ForecastSourcingItems>> GetAllForecastSourcingItems();
        Task<ForecastSourcingItems> GetForecastSourcingItemsById(int id);
        Task<int?> CreateForecastSourcingItems(ForecastSourcingItems forecastSourcingItems);
        Task<string> UpdateForecastSourcingItems(ForecastSourcingItems forecastSourcingItems);
        Task<string> DeleteForecastSourcingItems(ForecastSourcingItems forecastSourcingItems);
    }
}
