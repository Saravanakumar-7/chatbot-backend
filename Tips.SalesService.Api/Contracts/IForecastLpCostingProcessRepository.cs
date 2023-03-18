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
    public interface IForecastLpCostingProcessRepository
    {
        Task<PagedList<ForecastLpCostingProcess>> GetAllForecastLpCostingProcess(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<ForecastLpCostingProcess> GetForecastLpCostingProcessById(int id);
        Task<int?> CreateForecastLpCostingProcess(ForecastLpCostingProcess forecastLpCostingProcess);
        Task<string> UpdateForecastLpCostingProcess(ForecastLpCostingProcess forecastLpCostingProcess);
        Task<string> DeleteForecastLpCostingProcess(ForecastLpCostingProcess forecastLpCostingProcess);
    }
}
