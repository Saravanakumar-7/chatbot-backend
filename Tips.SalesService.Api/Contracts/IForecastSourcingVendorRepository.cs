using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForecastSourcingVendorRepository
    {
        Task<PagedList<ForecastSourcingVendor>> GetAllForecastSourcingVendor(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<ForecastSourcingVendor> GetForecastSourcingVendorById(int id);
        Task<int?> CreateForecastSourcingVendor(ForecastSourcingVendor forecastSourcingVendor);
        Task<string> UpdateForecastSourcingVendor(ForecastSourcingVendor forecastSourcingVendor);
        Task<string> DeleteForecastSourcingVendor(ForecastSourcingVendor forecastSourcingVendor);
    }
}
