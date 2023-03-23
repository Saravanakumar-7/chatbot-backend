using Entities.Helper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForecastLpCostingNREConsumableRepository
    {
        Task<PagedList<ForecastLPCostingNREConsumable>> GetAllForecastLPCostingNREConsumable(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<ForecastLPCostingNREConsumable> GetForecastLPCostingNREConsumableById(int id);
        Task<int?> CreateForecastLPCostingNREConsumable(ForecastLPCostingNREConsumable forecastLPCostingNREConsumable);
        Task<string> UpdateForecastLPCostingNREConsumable(ForecastLPCostingNREConsumable forecastLPCostingNREConsumable);
        Task<string> DeleteForecastLPCostingNREConsumable(ForecastLPCostingNREConsumable forecastLPCostingNREConsumable);
    }
}
