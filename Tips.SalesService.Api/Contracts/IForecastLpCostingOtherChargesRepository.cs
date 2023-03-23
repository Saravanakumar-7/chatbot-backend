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
    public interface IForecastLpCostingOtherChargesRepository
    {
        Task<PagedList<ForecastLpCostingOtherCharges>> GetAllForecastLpCostingOtherCharges(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<ForecastLpCostingOtherCharges> GetForecastLpCostingOtherChargesById(int id);
        Task<int?> CreateForecastLpCostingOtherCharges(ForecastLpCostingOtherCharges forecastLpCostingOtherCharges);
        Task<string> UpdateForecastLpCostingOtherCharges(ForecastLpCostingOtherCharges forecastLpCostingOtherCharges);
        Task<string> DeleteForecastLpCostingOtherCharges(ForecastLpCostingOtherCharges forecastLpCostingOtherCharges);
    }
}
