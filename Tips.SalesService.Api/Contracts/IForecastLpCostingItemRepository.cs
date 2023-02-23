using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForecastLpCostingItemRepository
    {
        Task<IEnumerable<ForecastLpCostingItem>> GetAllForecastLpCostingItem();
        Task<ForecastLpCostingItem> GetForecastLpCostingItemById(int id);
        Task<int?> CreateForecastLpCostingItem(ForecastLpCostingItem forecastLpCostingItem);
        Task<string> UpdateForecastLpCostingItem(ForecastLpCostingItem forecastLpCostingItem);
        Task<string> DeleteForecastLpCostingItem(ForecastLpCostingItem forecastLpCostingItem);
    }
}
